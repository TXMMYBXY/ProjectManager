import React, { useState } from 'react';
import { employeeApi } from '../../api/client';
import { UserRole, UserRoleLabel } from '../../api/types';
import { Modal } from '../../components/ui/Modal';
import { Input, Select } from '../../components/ui/Input';
import { Button } from '../../components/ui/Button';
import { useToast } from '../../components/ui/Toast';

interface Props {
  isOpen: boolean;
  onClose: () => void;
  onCreated: () => void;
}

export const CreateEmployeeModal: React.FC<Props> = ({ isOpen, onClose, onCreated }) => {
  const { toast } = useToast();
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    patronymic: '',
    email: '',
    password: '',
    role: '',
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const validate = () => {
    const e: Record<string, string> = {};
    if (!form.firstName.trim()) e.firstName = 'Required';
    if (!form.lastName.trim()) e.lastName = 'Required';
    if (!form.email.trim()) e.email = 'Required';
    if (form.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) e.email = 'Invalid email';
    if (!form.password) e.password = 'Required';
    return e;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const errs = validate();
    setErrors(errs);
    if (Object.keys(errs).length > 0) return;

    setLoading(true);
    try {
      await employeeApi.create({
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        patronymic: form.patronymic.trim() || undefined,
        email: form.email.trim(),
        password: form.password,
        role: form.role || undefined,
      });
      toast('success', 'Employee created successfully');
      onCreated();
      onClose();
      setForm({ firstName: '', lastName: '', patronymic: '', email: '', password: '', role: '' });
      setErrors({});
    } catch (err: any) {
      toast('error', err.message || 'Failed to create employee');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Add Employee" size="md">
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <div className="grid grid-cols-2 gap-3">
          <Input
            label="First Name"
            required
            value={form.firstName}
            onChange={(e) => setForm((f) => ({ ...f, firstName: e.target.value }))}
            error={errors.firstName}
          />
          <Input
            label="Last Name"
            required
            value={form.lastName}
            onChange={(e) => setForm((f) => ({ ...f, lastName: e.target.value }))}
            error={errors.lastName}
          />
        </div>
        <Input
          label="Patronymic"
          value={form.patronymic}
          onChange={(e) => setForm((f) => ({ ...f, patronymic: e.target.value }))}
        />
        <Input
          label="Email"
          type="email"
          required
          value={form.email}
          onChange={(e) => setForm((f) => ({ ...f, email: e.target.value }))}
          error={errors.email}
        />
        <Input
          label="Password"
          type="password"
          required
          value={form.password}
          onChange={(e) => setForm((f) => ({ ...f, password: e.target.value }))}
          error={errors.password}
        />
        <Select
          label="Role"
          value={form.role}
          onChange={(e) => setForm((f) => ({ ...f, role: e.target.value }))}
          placeholder="Select role (optional)"
          options={[
            { value: 'Director', label: 'Director' },
            { value: 'Manager', label: 'Manager' },
            { value: 'Employee', label: 'Employee' },
          ]}
        />
        <div className="flex gap-3 pt-2">
          <Button type="button" variant="outline" className="flex-1" onClick={onClose}>
            Cancel
          </Button>
          <Button type="submit" className="flex-1" loading={loading}>
            Create Employee
          </Button>
        </div>
      </form>
    </Modal>
  );
};
