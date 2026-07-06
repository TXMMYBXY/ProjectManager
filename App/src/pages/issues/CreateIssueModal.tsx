import React, { useState, useEffect } from 'react';
import { issueApi, projectApi, employeeApi } from '../../api/client';
import { IssueStatus } from '../../api/types';
import { Modal } from '../../components/ui/Modal';
import { Input, Select, Textarea } from '../../components/ui/Input';
import { Button } from '../../components/ui/Button';
import { useToast } from '../../components/ui/Toast';

interface Props {
  isOpen: boolean;
  onClose: () => void;
  onCreated: () => void;
  defaultProjectId?: number;
  defaultExecutorId?: number;
}

export const CreateIssueModal: React.FC<Props> = ({ isOpen, onClose, onCreated, defaultProjectId, defaultExecutorId }) => {
  const { toast } = useToast();
  const [loading, setLoading] = useState(false);
  const [projects, setProjects] = useState<{ id: number; title: string }[]>([]);
  const [employees, setEmployees] = useState<{ id: number; name: string }[]>([]);
  const [form, setForm] = useState({
    title: '',
    status: String(IssueStatus.ToDo),
    comments: '',
    priority: '5',
    projectId: defaultProjectId ? String(defaultProjectId) : '',
    executorId: defaultExecutorId ? String(defaultExecutorId) : '',
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (isOpen) {
      projectApi.list({ PageSize: 200 }).then((r) => setProjects((r.projects ?? []).map((p) => ({ id: p.id, title: p.title })))).catch(() => {});
      employeeApi.list({ PageSize: 200 }).then((r) => setEmployees((r.employees ?? []).map((e) => ({ id: e.id, name: `${e.lastName} ${e.firstName}` })))).catch(() => {});
      setForm(f => ({
        ...f,
        projectId: defaultProjectId ? String(defaultProjectId) : '',
        executorId: defaultExecutorId ? String(defaultExecutorId) : '',
      }));
    }
  }, [isOpen, defaultProjectId, defaultExecutorId]);

  const validate = () => {
    const e: Record<string, string> = {};
    if (!form.title.trim()) e.title = 'Required';
    if (!form.projectId) e.projectId = 'Required';
    if (!form.executorId) e.executorId = 'Required';
    return e;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const errs = validate();
    setErrors(errs);
    if (Object.keys(errs).length > 0) return;
    setLoading(true);
    try {
      await issueApi.create({
        title: form.title.trim(),
        status: Number(form.status) as IssueStatus,
        comments: form.comments || null,
        priority: Number(form.priority),
        projectId: Number(form.projectId),
        executorId: Number(form.executorId),
      });
      toast('success', 'Issue created successfully');
      onCreated();
      onClose();
      setForm({ title: '', status: String(IssueStatus.ToDo), comments: '', priority: '5', projectId: '', executorId: '' });
      setErrors({});
    } catch (err: any) {
      toast('error', err.message || 'Failed to create issue');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Create Issue" size="lg">
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <Input
          label="Title"
          required
          value={form.title}
          onChange={(e) => setForm((f) => ({ ...f, title: e.target.value }))}
          error={errors.title}
        />
        <div className="grid grid-cols-2 gap-3">
          <Select
            label="Status"
            value={form.status}
            onChange={(e) => setForm((f) => ({ ...f, status: e.target.value }))}
            options={[
              { value: IssueStatus.ToDo, label: 'To Do' },
              { value: IssueStatus.InProgress, label: 'In Progress' },
              { value: IssueStatus.Done, label: 'Done' },
            ]}
          />
          <div className="flex flex-col">
            <label className="text-sm font-medium text-gray-700 mb-1">Priority: {form.priority}</label>
            <input
              type="range"
              min="1"
              max="10"
              value={form.priority}
              onChange={(e) => setForm((f) => ({ ...f, priority: e.target.value }))}
              className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer"
            />
          </div>
        </div>
        <Select
          label="Project"
          required
          value={form.projectId}
          onChange={(e) => setForm((f) => ({ ...f, projectId: e.target.value }))}
          placeholder="Select project..."
          options={projects.map((p) => ({ value: p.id, label: p.title }))}
          error={errors.projectId}
          disabled={!!defaultProjectId}
        />
        <Select
          label="Executor"
          required
          value={form.executorId}
          onChange={(e) => setForm((f) => ({ ...f, executorId: e.target.value }))}
          placeholder="Select executor..."
          options={employees.map((e) => ({ value: e.id, label: e.name }))}
          error={errors.executorId}
          disabled={!!defaultExecutorId}
        />
        <Textarea
          label="Comments"
          value={form.comments}
          onChange={(e) => setForm((f) => ({ ...f, comments: e.target.value }))}
          rows={3}
        />
        <div className="flex gap-3 pt-2">
          <Button type="button" variant="outline" className="flex-1" onClick={onClose}>Cancel</Button>
          <Button type="submit" className="flex-1" loading={loading}>Create Issue</Button>
        </div>
      </form>
    </Modal>
  );
};
