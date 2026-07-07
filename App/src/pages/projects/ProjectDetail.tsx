import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { ArrowLeft, Edit2, Trash2, Save, X, Users, CircleDot, Plus, UserMinus } from 'lucide-react';
import { projectApi, employeeApi } from '../../api/client';
import { ProjectInfoResponse, EmployeeItemDto, IssueStatus, IssueStatusLabel } from '../../api/types';
import { Button } from '../../components/ui/Button';
import { Input, Select } from '../../components/ui/Input';
import { Badge } from '../../components/ui/Badge';
import { ConfirmDialog } from '../../components/ui/ConfirmDialog';
import { Modal } from '../../components/ui/Modal';
import { useToast } from '../../components/ui/Toast';
import { CreateIssueModal } from '../issues/CreateIssueModal';

const statusVariant = (s: IssueStatus) => {
  if (s === IssueStatus.Done) return 'success';
  if (s === IssueStatus.InProgress) return 'info';
  return 'default';
};

const priorityVariant = (p: number): 'danger' | 'warning' | 'success' => {
  if (p >= 8) return 'danger';
  if (p >= 5) return 'warning';
  return 'success';
};

const fmt = (d?: string | null) => {
  if (!d) return '—';
  return new Date(d).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
};

const toDateInput = (d?: string | null) => {
  if (!d) return '';
  return new Date(d).toISOString().split('T')[0];
};

export const ProjectDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { toast } = useToast();

  const [project, setProject] = useState<ProjectInfoResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [showAddEmployee, setShowAddEmployee] = useState(false);
  const [showCreateIssue, setShowCreateIssue] = useState(false);
  const [allEmployees, setAllEmployees] = useState<EmployeeItemDto[]>([]);
  const [addingEmployeeIds, setAddingEmployeeIds] = useState<Set<number>>(new Set());
  const [removingEmpId, setRemovingEmpId] = useState<number | null>(null);
  const [addLoading, setAddLoading] = useState(false);
  const [removeLoading, setRemoveLoading] = useState(false);
  const [selectedEmployees, setSelectedEmployees] = useState<Set<number>>(new Set());
  const [bulkRemoving, setBulkRemoving] = useState(false);
  const [bulkRemoveLoading, setBulkRemoveLoading] = useState(false);

  const [editForm, setEditForm] = useState({
    title: '',
    companyCustomer: '',
    companyExecuter: '',
    finishDate: '',
    priority: '',
    projectManagerId: '',
  });

  const load = async () => {
    if (!id) return;
    setLoading(true);
    try {
      const res = await projectApi.get(Number(id));
      setProject(res);
      setEditForm({
        title: res.title,
        companyCustomer: res.companyCustomer,
        companyExecuter: res.companyExecuter,
        finishDate: toDateInput(res.finishDate),
        priority: String(res.priority),
        projectManagerId: res.projectManager ? String(res.projectManager.id) : '',
      });
    } catch (e: any) {
      toast('error', e.message || 'Failed to load project');
    } finally {
      setLoading(false);
    }
  };

  const loadEmployees = async () => {
    try {
      // load only eligible project managers/directors for manager select
      const res = await employeeApi.getProjectManagers();
      setAllEmployees(res.projectManagers ?? []);
    } catch { }
  };

  useEffect(() => { load(); loadEmployees(); }, [id]);

  const handleSave = async () => {
    if (!id) return;
    setSaving(true);
    try {
      // send only changed fields
      const body: any = {};
      if ((project?.title ?? '') !== editForm.title) body.title = editForm.title || null;
      if ((project?.companyCustomer ?? '') !== editForm.companyCustomer) body.companyCustomer = editForm.companyCustomer || null;
      if ((project?.companyExecuter ?? '') !== editForm.companyExecuter) body.companyExecuter = editForm.companyExecuter || null;
      const originalFinish = toDateInput(project?.finishDate);
      if (originalFinish !== editForm.finishDate) body.finishDate = { hasValue: true, value: editForm.finishDate ? new Date(editForm.finishDate).toISOString() : null };
      if (String(project?.priority ?? '') !== editForm.priority) body.priority = editForm.priority ? Number(editForm.priority) : null;
      const originalManager = project?.projectManager ? String(project.projectManager.id) : '';
      if (originalManager !== editForm.projectManagerId) body.projectManagerId = editForm.projectManagerId ? Number(editForm.projectManagerId) : null;

      if (Object.keys(body).length === 0) {
        toast('info', 'No changes to save');
        setEditing(false);
        return;
      }

      await projectApi.update(Number(id), body);
      toast('success', 'Project updated');
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
      await projectApi.delete(Number(id));
      toast('success', 'Project deleted');
      navigate('/projects');
    } catch (e: any) {
      toast('error', e.message || 'Delete failed');
    } finally {
      setDeleteLoading(false);
    }
  };

  const handleBulkAddEmployee = async () => {
    if (!id || addingEmployeeIds.size === 0) return;
    setAddLoading(true);
    try {
      await projectApi.bulkInsertEmployees(Number(id), { ids: Array.from(addingEmployeeIds) });
      toast('success', `${addingEmployeeIds.size} employees added`);
      setShowAddEmployee(false);
      setAddingEmployeeIds(new Set());
      load();
    } catch (e: any) {
      toast('error', e.message || 'Failed to add employees');
    } finally {
      setAddLoading(false);
    }
  };

  const handleRemoveEmployee = async () => {
    if (!id || !removingEmpId) return;
    setRemoveLoading(true);
    try {
      await projectApi.removeEmployee(Number(id), removingEmpId);
      toast('success', 'Employee removed');
      setRemovingEmpId(null);
      load();
    } catch (e: any) {
      toast('error', e.message || 'Failed to remove employee');
    } finally {
      setRemoveLoading(false);
    }
  };

  const handleSelectEmployee = (empId: number) => {
    setSelectedEmployees(prev => {
      const next = new Set(prev);
      if (next.has(empId)) {
        next.delete(empId);
      } else {
        next.add(empId);
      }
      return next;
    });
  };

  const handleSelectAll = () => {
    if (project && selectedEmployees.size === project.employees?.length) {
      setSelectedEmployees(new Set());
    } else {
      setSelectedEmployees(new Set(project?.employees?.map(e => e.id) ?? []));
    }
  };

  const handleBulkRemove = async () => {
    if (!id || selectedEmployees.size === 0) return;
    setBulkRemoveLoading(true);
    try {
      await projectApi.bulkRemoveEmployees(Number(id), { ids: Array.from(selectedEmployees) });
      toast('success', `${selectedEmployees.size} employees removed`);
      setBulkRemoving(false);
      setSelectedEmployees(new Set());
      load();
    } catch (e: any) {
      toast('error', e.message || 'Failed to remove employees');
    } finally {
      setBulkRemoveLoading(false);
    }
  };

  const handleToggleAddEmployee = (empId: number) => {
    setAddingEmployeeIds(prev => {
      const next = new Set(prev);
      if (next.has(empId)) {
        next.delete(empId);
      } else {
        next.add(empId);
      }
      return next;
    });
  };

  if (loading) {
    return (
      <div className="p-8 animate-pulse space-y-4">
        <div className="h-8 bg-gray-200 rounded w-64" />
        <div className="h-48 bg-gray-200 rounded-2xl" />
      </div>
    );
  }

  if (!project) {
    return (
      <div className="p-8 text-center">
        <p className="text-gray-400">Project not found</p>
        <Button variant="ghost" onClick={() => navigate('/projects')} className="mt-4">
          <ArrowLeft size={16} /> Go back
        </Button>
      </div>
    );
  }

  const projectEmployeeIds = new Set((project.employees ?? []).map((e) => e.id));
  const availableEmployees = allEmployees.filter((e) => !projectEmployeeIds.has(e.id));

  return (
    <div className="p-8">
      {/* Header */}
      <div className="flex items-start gap-4 mb-6">
        <Button variant="ghost" size="sm" onClick={() => navigate('/projects')}>
          <ArrowLeft size={16} /> Back
        </Button>
        <div className="flex-1">
          <h1 className="text-2xl font-bold text-gray-900">{project.title}</h1>
          <p className="text-gray-500 text-sm mt-0.5">{project.companyCustomer} → {project.companyExecuter}</p>
        </div>
        <div className="flex gap-2">
          {editing ? (
            <>
              <Button variant="outline" size="sm" onClick={() => setEditing(false)}><X size={15} /> Cancel</Button>
              <Button size="sm" loading={saving} onClick={handleSave}><Save size={15} /> Save</Button>
            </>
          ) : (
            <>
              <Button variant="outline" size="sm" onClick={() => setEditing(true)}><Edit2 size={15} /> Edit</Button>
              <Button variant="danger" size="sm" onClick={() => setDeleting(true)}><Trash2 size={15} /> Delete</Button>
            </>
          )}
        </div>
      </div>

      {/* Project Info */}
      <div className="bg-white rounded-2xl border border-gray-100 shadow-sm p-6 mb-6">
        <h2 className="text-base font-semibold text-gray-700 mb-4">Project Details</h2>
        {editing ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <Input label="Title" value={editForm.title} onChange={(e) => setEditForm((f) => ({ ...f, title: e.target.value }))} />
            <Input label="Company Customer" value={editForm.companyCustomer} onChange={(e) => setEditForm((f) => ({ ...f, companyCustomer: e.target.value }))} />
            <Input label="Company Executor" value={editForm.companyExecuter} onChange={(e) => setEditForm((f) => ({ ...f, companyExecuter: e.target.value }))} />
            <Input label="Priority (1-10)" type="number" min="1" max="10" value={editForm.priority} onChange={(e) => setEditForm((f) => ({ ...f, priority: e.target.value }))} />
            <Input label="Finish Date" type="date" value={editForm.finishDate} onChange={(e) => setEditForm((f) => ({ ...f, finishDate: e.target.value }))} />
            <Select
              label="Project Manager"
              value={editForm.projectManagerId}
              onChange={(e) => setEditForm((f) => ({ ...f, projectManagerId: e.target.value }))}
              placeholder="No manager"
              options={allEmployees.map((m) => ({ value: m.id, label: `${m.lastName} ${m.firstName}` }))}
            />
          </div>
        ) : (
          <div className="grid grid-cols-2 sm:grid-cols-3 gap-4">
            {[
              { label: 'Customer', value: project.companyCustomer },
              { label: 'Executor', value: project.companyExecuter },
              { label: 'Priority', value: <Badge variant={priorityVariant(project.priority)}>{project.priority}</Badge> },
              { label: 'Start Date', value: fmt(project.startDate) },
              { label: 'Finish Date', value: fmt(project.finishDate) },
              {
                label: 'Manager',
                value: project.projectManager
                  ? `${project.projectManager.lastName} ${project.projectManager.firstName}`
                  : '—',
              },
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
        {/* Employees */}
        <div className="bg-white rounded-2xl border border-gray-100 shadow-sm">
          <div className="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
            <div className="flex items-center gap-2">
              <Users size={16} className="text-indigo-500" />
              <h2 className="font-semibold text-gray-800 text-sm">
                Employees ({project.employees?.length ?? 0})
              </h2>
            </div>
            <div className="flex items-center gap-2">
              {selectedEmployees.size > 0 && (
                <Button size="sm" variant="danger" onClick={() => setBulkRemoving(true)}>
                  <Trash2 size={14} /> Remove ({selectedEmployees.size})
                </Button>
              )}
              <Button size="sm" variant="ghost" onClick={() => setShowAddEmployee(true)}>
                <Plus size={14} /> Add
              </Button>
            </div>
          </div>
          <div className="divide-y divide-gray-50 max-h-72 overflow-y-auto">
            {project.employees && project.employees.length > 1 && (
              <div className="flex items-center px-5 py-2 bg-gray-50">
                <input
                  type="checkbox"
                  className="mr-4 h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
                  checked={project.employees.length > 0 && selectedEmployees.size === project.employees.length}
                  onChange={handleSelectAll}
                />
                <span className="text-xs text-gray-500 font-medium">Select all</span>
              </div>
            )}
            {(project.employees ?? []).length === 0 ? (
              <p className="px-5 py-6 text-center text-gray-400 text-sm">No employees assigned</p>
            ) : (
              project.employees!.map((emp) => (
                <div key={emp.id} className="flex items-center justify-between px-5 py-3">
                  <input
                    type="checkbox"
                    className="mr-4 h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
                    checked={selectedEmployees.has(emp.id)}
                    onChange={() => handleSelectEmployee(emp.id)}
                  />
                  <Link to={`/employees/${emp.id}`} className="flex-1 hover:text-indigo-600 transition-colors">
                    <p className="text-sm font-medium text-gray-800">{emp.lastName} {emp.firstName}</p>
                    <p className="text-xs text-gray-400">{emp.email}</p>
                  </Link>
                  <button
                    onClick={() => setRemovingEmpId(emp.id)}
                    className="p-1.5 rounded-lg text-gray-400 hover:text-red-600 hover:bg-red-50 transition-colors"
                  >
                    <UserMinus size={14} />
                  </button>
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
                Issues ({project.issues?.length ?? 0})
              </h2>
            </div>
            <div>
              <Button size="sm" variant="ghost" onClick={() => setShowCreateIssue(true)} className="mr-2">
                <Plus size={14} /> New Issue
              </Button>
              <Link to={`/issues?projectId=${id}`} className="text-xs text-indigo-600 hover:text-indigo-700">View all</Link>
            </div>
          </div>
          <div className="divide-y divide-gray-50 max-h-72 overflow-y-auto">
            {(project.issues ?? []).length === 0 ? (
              <p className="px-5 py-6 text-center text-gray-400 text-sm">No issues</p>
            ) : (
              project.issues!.map((issue) => (
                <Link key={issue.id} to={`/issues/${issue.id}`} className="flex items-center justify-between px-5 py-3 hover:bg-gray-50 transition-colors">
                  <div>
                    <p className="text-sm font-medium text-gray-800">{issue.title}</p>
                    <p className="text-xs text-gray-400">
                      {issue.executor ? `${issue.executor.lastName} ${issue.executor.firstName}` : 'Unassigned'}
                    </p>
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

      {/* Add Employee Modal */}
      <Modal isOpen={showAddEmployee} onClose={() => setShowAddEmployee(false)} title="Add Employees to Project">
        <div className="flex flex-col gap-4">
          <div className="max-h-64 overflow-y-auto border rounded-lg p-2">
            {availableEmployees.length === 0 ? (
              <p className="text-center text-gray-400 text-sm py-4">No employees available to add</p>
            ) : (
              availableEmployees.map(e => (
                <label key={e.id} className="flex items-center p-2 rounded-md hover:bg-gray-100 cursor-pointer">
                  <input
                    type="checkbox"
                    className="mr-3 h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
                    checked={addingEmployeeIds.has(e.id)}
                    onChange={() => handleToggleAddEmployee(e.id)}
                  />
                  <span>{e.lastName} {e.firstName} — {e.email}</span>
                </label>
              ))
            )}
          </div>
          <div className="flex gap-3">
            <Button variant="outline" className="flex-1" onClick={() => { setShowAddEmployee(false); setAddingEmployeeIds(new Set()); }}>Cancel</Button>
            <Button className="flex-1" loading={addLoading} disabled={addingEmployeeIds.size === 0} onClick={handleBulkAddEmployee}>Add Selected ({addingEmployeeIds.size})</Button>
          </div>
        </div>
      </Modal>

      <CreateIssueModal
        isOpen={showCreateIssue}
        onClose={() => setShowCreateIssue(false)}
        onCreated={() => { setShowCreateIssue(false); load(); }}
        defaultProjectId={Number(id)}
      />

      <ConfirmDialog
        isOpen={deleting}
        onClose={() => setDeleting(false)}
        onConfirm={handleDelete}
        title="Delete Project"
        message={`Are you sure you want to delete "${project.title}"?`}
        loading={deleteLoading}
      />

      <ConfirmDialog
        isOpen={!!removingEmpId}
        onClose={() => setRemovingEmpId(null)}
        onConfirm={handleRemoveEmployee}
        title="Remove Employee"
        message="Remove this employee from the project?"
        loading={removeLoading}
      />

      <ConfirmDialog
        isOpen={bulkRemoving}
        onClose={() => setBulkRemoving(false)}
        onConfirm={handleBulkRemove}
        title="Remove Employees"
        message={`Remove ${selectedEmployees.size} selected employees from the project?`}
        loading={bulkRemoveLoading}
      />
    </div>
  );
};
