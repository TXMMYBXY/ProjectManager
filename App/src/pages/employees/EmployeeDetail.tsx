import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { ArrowLeft, Edit2, Trash2, FolderKanban, CircleDot, Save, X, Plus, UserMinus } from 'lucide-react';
import { employeeApi, projectApi } from '../../api/client';
import { EmployeeInfoResponse, IssueStatus, IssueStatusLabel, UserRole, ProjectItemDto } from '../../api/types';
import { Button } from '../../components/ui/Button';
import { Input, Select } from '../../components/ui/Input';
import { Badge } from '../../components/ui/Badge';
import { ConfirmDialog } from '../../components/ui/ConfirmDialog';
import { useToast } from '../../components/ui/Toast';
import { Modal } from '../../components/ui/Modal';
import { CreateIssueModal } from '../issues/CreateIssueModal';

const statusVariant = (s: IssueStatus) => {
  if (s === IssueStatus.Done) return 'success';
  if (s === IssueStatus.InProgress) return 'info';
  return 'default';
};

const priorityVariant = (p: number) => {
  if (p >= 8) return 'danger';
  if (p >= 5) return 'warning';
  return 'success';
};

export const EmployeeDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { toast } = useToast();

  const [employee, setEmployee] = useState<EmployeeInfoResponse | null>(null);
  const [allProjects, setAllProjects] = useState<ProjectItemDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [showAddProject, setShowAddProject] = useState(false);
  const [showCreateIssue, setShowCreateIssue] = useState(false);
  const [addLoading, setAddLoading] = useState(false);
  const [addingProjectIds, setAddingProjectIds] = useState<Set<number>>(new Set());
  const [selectedProjects, setSelectedProjects] = useState<Set<number>>(new Set());
  const [bulkRemoving, setBulkRemoving] = useState(false);
  const [bulkRemoveLoading, setBulkRemoveLoading] = useState(false);

  const [editForm, setEditForm] = useState({
    firstName: '',
    lastName: '',
    patronymic: '',
    email: '',
    role: '',
  });

  const load = async () => {
    if (!id) return;
    setLoading(true);
    try {
      const [emp, projRes] = await Promise.all([
        employeeApi.get(Number(id)),
        projectApi.list({ PageSize: 500 }),
      ]);
      setEmployee(emp);
      setAllProjects(projRes.projects ?? []);
      setEditForm({
        firstName: emp.firstName,
        lastName: emp.lastName,
        patronymic: emp.patronymic ?? '',
        email: emp.email,
        role: '',
      });
    } catch (e: any) {
      toast('error', e.message || 'Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, [id]);

  const handleSave = async () => {
    if (!id) return;
    setSaving(true);
    try {
      await employeeApi.update(Number(id), {
        firstName: editForm.firstName || null,
        lastName: editForm.lastName || null,
        patronymic: { hasValue: true, value: editForm.patronymic || null },
        email: editForm.email || null,
        role: editForm.role || null,
      });
      toast('success', 'Employee updated');
      setEditing(false);
      load();
    } catch (e: any) {
      toast('error', e.message || 'Update failed');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!id) return;
    setDeleteLoading(true);
    try {
      await employeeApi.delete(Number(id));
      toast('success', 'Employee deleted');
      navigate('/employees');
    } catch (e: any) {
      toast('error', e.message || 'Delete failed');
    } finally {
      setDeleteLoading(false);
    }
  };

  const handleBulkAdd = async () => {
    if (!id || addingProjectIds.size === 0) return;
    setAddLoading(true);
    try {
      await employeeApi.bulkInsertToProject(Number(id), { ids: Array.from(addingProjectIds) });
      toast('success', `Added to ${addingProjectIds.size} projects`);
      setShowAddProject(false);
      setAddingProjectIds(new Set());
      load();
    } catch (e: any) {
      toast('error', e.message || 'Failed to add to projects');
    } finally {
      setAddLoading(false);
    }
  };

  const handleBulkRemove = async () => {
    if (!id || selectedProjects.size === 0) return;
    setBulkRemoveLoading(true);
    try {
      await employeeApi.bulkRemoveFromProject(Number(id), { ids: Array.from(selectedProjects) });
      toast('success', `Removed from ${selectedProjects.size} projects`);
      setBulkRemoving(false);
      setSelectedProjects(new Set());
      load();
    } catch (e: any) {
      toast('error', e.message || 'Failed to remove from projects');
    } finally {
      setBulkRemoveLoading(false);
    }
  };

  const handleToggleAddProject = (projId: number) => {
    setAddingProjectIds(prev => {
      const next = new Set(prev);
      if (next.has(projId)) next.delete(projId);
      else next.add(projId);
      return next;
    });
  };

  const handleSelectProject = (projId: number) => {
    setSelectedProjects(prev => {
      const next = new Set(prev);
      if (next.has(projId)) next.delete(projId);
      else next.add(projId);
      return next;
    });
  };

  const handleSelectAll = () => {
    if (employee && selectedProjects.size === employee.projects?.length) {
      setSelectedProjects(new Set());
    } else {
      setSelectedProjects(new Set(employee?.projects?.map(p => p.id) ?? []));
    }
  };

  if (loading) {
    return (
      <div className="p-8">
        <div className="animate-pulse space-y-4">
          <div className="h-8 bg-gray-200 rounded w-48" />
          <div className="h-48 bg-gray-200 rounded-2xl" />
        </div>
      </div>
    );
  }

  if (!employee) {
    return (
      <div className="p-8 text-center">
        <p className="text-gray-400">Employee not found</p>
        <Button variant="ghost" onClick={() => navigate('/employees')} className="mt-4">
          <ArrowLeft size={16} /> Go back
        </Button>
      </div>
    );
  }

  const assignedProjectIds = new Set(employee.projects?.map(p => p.id));
  const availableProjects = allProjects.filter(p => !assignedProjectIds.has(p.id));
  const uniqueIssues = [...new Map([...(employee.authoredIssues ?? []), ...(employee.executedIssues ?? [])].map(item => [item.id, item])).values()];

  return (
    <div className="p-8">
      <div className="flex items-center gap-4 mb-6">
        <Button variant="ghost" size="sm" onClick={() => navigate('/employees')}>
          <ArrowLeft size={16} /> Back
        </Button>
        <div className="flex-1">
          <h1 className="text-2xl font-bold text-gray-900">
            {employee.lastName} {employee.firstName} {employee.patronymic ?? ''}
          </h1>
          <p className="text-gray-500 text-sm mt-0.5">{employee.email}</p>
        </div>
        <div className="flex gap-2">
          {editing ? (
            <>
              <Button variant="outline" size="sm" onClick={() => setEditing(false)}>
                <X size={15} /> Cancel
              </Button>
              <Button size="sm" loading={saving} onClick={handleSave}>
                <Save size={15} /> Save
              </Button>
            </>
          ) : (
            <>
              <Button variant="outline" size="sm" onClick={() => setEditing(true)}>
                <Edit2 size={15} /> Edit
              </Button>
              <Button variant="danger" size="sm" onClick={() => setDeleting(true)}>
                <Trash2 size={15} /> Delete
              </Button>
            </>
          )}
        </div>
      </div>

      {/* Info Card */}
      <div className="bg-white rounded-2xl border border-gray-100 shadow-sm p-6 mb-6">
        <h2 className="text-base font-semibold text-gray-700 mb-4">Personal Information</h2>
        {editing ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <Input
              label="First Name"
              value={editForm.firstName}
              onChange={(e) => setEditForm((f) => ({ ...f, firstName: e.target.value }))}
            />
            <Input
              label="Last Name"
              value={editForm.lastName}
              onChange={(e) => setEditForm((f) => ({ ...f, lastName: e.target.value }))}
            />
            <Input
              label="Patronymic"
              value={editForm.patronymic}
              onChange={(e) => setEditForm((f) => ({ ...f, patronymic: e.target.value }))}
            />
            <Input
              label="Email"
              type="email"
              value={editForm.email}
              onChange={(e) => setEditForm((f) => ({ ...f, email: e.target.value }))}
            />
            <Select
              label="Role"
              value={editForm.role}
              onChange={(e) => setEditForm((f) => ({ ...f, role: e.target.value }))}
              placeholder="Keep existing role"
              options={[
                { value: 'Director', label: 'Director' },
                { value: 'Manager', label: 'Manager' },
                { value: 'Employee', label: 'Employee' },
              ]}
            />
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            {[
              { label: 'First Name', value: employee.firstName },
              { label: 'Last Name', value: employee.lastName },
              { label: 'Patronymic', value: employee.patronymic || '—' },
              { label: 'Email', value: employee.email },
            ].map(({ label, value }) => (
              <div key={label}>
                <p className="text-xs text-gray-400 font-medium uppercase tracking-wide mb-1">{label}</p>
                <p className="text-sm font-medium text-gray-800">{value}</p>
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Projects */}
        <div className="bg-white rounded-2xl border border-gray-100 shadow-sm">
          <div className="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
            <div className="flex items-center gap-2">
              <FolderKanban size={16} className="text-indigo-500" />
              <h2 className="font-semibold text-gray-800 text-sm">
                Projects ({employee.projects?.length ?? 0})
              </h2>
            </div>
            <div className="flex items-center gap-2">
              {selectedProjects.size > 0 && (
                <Button size="sm" variant="danger" onClick={() => setBulkRemoving(true)}>
                  <UserMinus size={14} /> Remove ({selectedProjects.size})
                </Button>
              )}
              <Button size="sm" variant="ghost" onClick={() => setShowAddProject(true)}>
                <Plus size={14} /> Add
              </Button>
            </div>
          </div>
          <div className="divide-y divide-gray-50 max-h-64 overflow-y-auto">
            {employee.projects && employee.projects.length > 1 && (
              <div className="flex items-center px-5 py-2 bg-gray-50">
                <input
                  type="checkbox"
                  className="mr-4 h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
                  checked={employee.projects.length > 0 && selectedProjects.size === employee.projects.length}
                  onChange={handleSelectAll}
                />
                <span className="text-xs text-gray-500 font-medium">Select all</span>
              </div>
            )}
            {(employee.projects ?? []).length === 0 ? (
              <p className="px-5 py-6 text-center text-gray-400 text-sm">No projects assigned</p>
            ) : (
              employee.projects!.map((p) => (
                <div key={p.id} className="flex items-center justify-between px-5 py-3">
                  <input
                    type="checkbox"
                    className="mr-4 h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
                    checked={selectedProjects.has(p.id)}
                    onChange={() => handleSelectProject(p.id)}
                  />
                  <Link to={`/projects/${p.id}`} className="flex-1 hover:text-indigo-600 transition-colors">
                    <div>
                      <p className="text-sm font-medium text-gray-800">{p.title}</p>
                      <p className="text-xs text-gray-400">{p.companyCustomer}</p>
                    </div>
                  </Link>
                  <Badge variant={priorityVariant(p.priority) as any}>{p.priority}</Badge>
                </div>
              ))
            )}
          </div>
        </div>

        {/* Issues */}
        <div className="bg-white rounded-2xl border border-gray-100 shadow-sm">
          <div className="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
            <div className="flex items-center gap-2">
              <CircleDot size={16} className="text-amber-500" />
              <h2 className="font-semibold text-gray-800 text-sm">
                Issues ({uniqueIssues.length})
              </h2>
            </div>
            <Button size="sm" variant="ghost" onClick={() => setShowCreateIssue(true)}>
              <Plus size={14} /> New Issue
            </Button>
          </div>
          <div className="divide-y divide-gray-50 max-h-64 overflow-y-auto">
            {uniqueIssues.length === 0 ? (
              <p className="px-5 py-6 text-center text-gray-400 text-sm">No issues</p>
            ) : (
              uniqueIssues.map((issue: any) => (
                <Link key={issue.id} to={`/issues/${issue.id}`} className="flex items-center justify-between px-5 py-3 hover:bg-gray-50 transition-colors">
                  <div>
                    <p className="text-sm font-medium text-gray-800">{issue.title}</p>
                    <p className="text-xs text-gray-400">{issue.project?.title ?? '—'}</p>
                  </div>
                  <Badge variant={statusVariant(issue.status)}>
                    {IssueStatusLabel[issue.status as IssueStatus]}
                  </Badge>
                </Link>
              ))
            )}
          </div>
        </div>
      </div>

      <Modal isOpen={showAddProject} onClose={() => setShowAddProject(false)} title="Add to Projects">
        <div className="flex flex-col gap-4">
          <div className="max-h-64 overflow-y-auto border rounded-lg p-2">
            {availableProjects.length === 0 ? (
              <p className="text-center text-gray-400 text-sm py-4">No other projects available</p>
            ) : (
              availableProjects.map(p => (
                <label key={p.id} className="flex items-center p-2 rounded-md hover:bg-gray-100 cursor-pointer">
                  <input
                    type="checkbox"
                    className="mr-3 h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
                    checked={addingProjectIds.has(p.id)}
                    onChange={() => handleToggleAddProject(p.id)}
                  />
                  <span>{p.title}</span>
                </label>
              ))
            )}
          </div>
          <div className="flex gap-3">
            <Button variant="outline" className="flex-1" onClick={() => { setShowAddProject(false); setAddingProjectIds(new Set()); }}>Cancel</Button>
            <Button className="flex-1" loading={addLoading} disabled={addingProjectIds.size === 0} onClick={handleBulkAdd}>Add to Selected ({addingProjectIds.size})</Button>
          </div>
        </div>
      </Modal>

      <CreateIssueModal
        isOpen={showCreateIssue}
        onClose={() => setShowCreateIssue(false)}
        onCreated={() => { setShowCreateIssue(false); load(); }}
        defaultExecutorId={Number(id)}
      />

      <ConfirmDialog
        isOpen={deleting}
        onClose={() => setDeleting(false)}
        onConfirm={handleDelete}
        title="Delete Employee"
        message={`Are you sure you want to delete ${employee.firstName} ${employee.lastName}?`}
        loading={deleteLoading}
      />

      <ConfirmDialog
        isOpen={bulkRemoving}
        onClose={() => setBulkRemoving(false)}
        onConfirm={handleBulkRemove}
        title="Remove from Projects"
        message={`Remove employee from ${selectedProjects.size} selected projects?`}
        loading={bulkRemoveLoading}
      />
    </div>
  );
};
