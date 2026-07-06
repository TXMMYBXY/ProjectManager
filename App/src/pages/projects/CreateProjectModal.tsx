import React, { useState, useEffect } from 'react';
import { projectApi, employeeApi } from '../../api/client';
import { Modal } from '../../components/ui/Modal';
import { Input, Select } from '../../components/ui/Input';
import { Button } from '../../components/ui/Button';
import { useToast } from '../../components/ui/Toast';
import { EmployeeItemDto, UserRole } from '../../api/types';

interface Props {
  isOpen: boolean;
  onClose: () => void;
  onCreated: () => void;
}

export const CreateProjectModal: React.FC<Props> = ({ isOpen, onClose, onCreated }) => {
  const { toast } = useToast();
  const [loading, setLoading] = useState(false);
  const [managers, setManagers] = useState<EmployeeItemDto[]>([]);
  const [form, setForm] = useState({
    title: '',
    companyCustomer: '',
    companyExecutor: '',
    startDate: '',
    finishDate: '',
    priority: '5',
    projectManagerId: '',
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (isOpen) {
      employeeApi.list({ PageSize: 100, role: UserRole.Manager }).then((r) => setManagers(r.employees ?? [])).catch(() => {});
    }
  }, [isOpen]);

  const validate = () => {
    const e: Record<string, string> = {};
    if (!form.title.trim()) e.title = 'Required';
    if (!form.companyCustomer.trim()) e.companyCustomer = 'Required';
    if (!form.companyExecutor.trim()) e.companyExecutor = 'Required';
    if (!form.startDate) e.startDate = 'Required';
    return e;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const errs = validate();
    setErrors(errs);
    if (Object.keys(errs).length > 0) return;

    setLoading(true);
    try {
      await projectApi.create({
        title: form.title.trim(),
        companyCustomer: form.companyCustomer.trim(),
        companyExecutor: form.companyExecutor.trim(),
        startDate: new Date(form.startDate).toISOString(),
        finishDate: form.finishDate ? new Date(form.finishDate).toISOString() : null,
        priority: Number(form.priority),
        projectManagerId: form.projectManagerId ? Number(form.projectManagerId) : null,
      });
      toast('success', 'Project created successfully');
      onCreated();
      onClose();
      setForm({ title: '', companyCustomer: '', companyExecutor: '', startDate: '', finishDate: '', priority: '5', projectManagerId: '' });
      setErrors({});
    } catch (err: any) {
      toast('error', err.message || 'Failed to create project');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Create Project" size="lg">
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <Input
          label="Title"
          required
          value={form.title}
          onChange={(e) => setForm((f) => ({ ...f, title: e.target.value }))}
          error={errors.title}
        />
        <div className="grid grid-cols-2 gap-3">
          <Input
            label="Company Customer"
            required
            value={form.companyCustomer}
            onChange={(e) => setForm((f) => ({ ...f, companyCustomer: e.target.value }))}
            error={errors.companyCustomer}
          />
          <Input
            label="Company Executor"
            required
            value={form.companyExecutor}
            onChange={(e) => setForm((f) => ({ ...f, companyExecutor: e.target.value }))}
            error={errors.companyExecutor}
          />
        </div>
        <div className="grid grid-cols-2 gap-3">
          <Input
            label="Start Date"
            type="date"
            required
            value={form.startDate}
            onChange={(e) => setForm((f) => ({ ...f, startDate: e.target.value }))}
            error={errors.startDate}
          />
          <Input
            label="Finish Date"
            type="date"
            value={form.finishDate}
            onChange={(e) => setForm((f) => ({ ...f, finishDate: e.target.value }))}
          />
        </div>
        <div className="grid grid-cols-2 gap-3">
          <Input
            label="Priority (0-10)"
            type="number"
            min="0"
            max="10"
            value={form.priority}
            onChange={(e) => setForm((f) => ({ ...f, priority: e.target.value }))}
          />
          <Select
            label="Project Manager"
            value={form.projectManagerId}
            onChange={(e) => setForm((f) => ({ ...f, projectManagerId: e.target.value }))}
            placeholder="No manager"
            options={managers.map((m) => ({
              value: m.id,
              label: `${m.lastName} ${m.firstName}`,
            }))}
          />
        </div>
        <div className="flex gap-3 pt-2">
          <Button type="button" variant="outline" className="flex-1" onClick={onClose}>
            Cancel
          </Button>
          <Button type="submit" className="flex-1" loading={loading}>
            Create Project
          </Button>
        </div>
      </form>
    </Modal>
  );
};
