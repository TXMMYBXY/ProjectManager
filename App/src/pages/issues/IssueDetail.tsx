import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { ArrowLeft, Edit2, Trash2, Save, X } from 'lucide-react';
import { issueApi, employeeApi } from '../../api/client';
import { IssueInfoResponse, IssueStatus, IssueStatusLabel } from '../../api/types';
import { Button } from '../../components/ui/Button';
import { Input, Select, Textarea } from '../../components/ui/Input';
import { Badge } from '../../components/ui/Badge';
import { ConfirmDialog } from '../../components/ui/ConfirmDialog';
import { useToast } from '../../components/ui/Toast';

const statusVariant = (s: IssueStatus): 'success' | 'info' | 'default' => {
  if (s === IssueStatus.Done) return 'success';
  if (s === IssueStatus.InProgress) return 'info';
  return 'default';
};

const priorityVariant = (p: number): 'danger' | 'warning' | 'success' => {
  if (p >= 8) return 'danger';
  if (p >= 5) return 'warning';
  return 'success';
};

export const IssueDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { toast } = useToast();

  const [issue, setIssue] = useState<IssueInfoResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [employees, setEmployees] = useState<{ id: number; name: string }[]>([]);

  const [editForm, setEditForm] = useState({
    title: '',
    status: '',
    comments: '',
    priority: '',
    executorId: '',
  });

  const load = async () => {
    if (!id) return;
    setLoading(true);
    try {
      const res = await issueApi.get(Number(id));
      setIssue(res);
      setEditForm({
        title: res.title,
        status: String(res.status),
        comments: res.comments ?? '',
        priority: String(res.priority),
        executorId: res.executor ? String(res.executor.id) : '',
      });
    } catch (e: any) {
      toast('error', e.message || 'Failed to load issue');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, [id]);

  useEffect(() => {
    employeeApi.list({ PageSize: 200 })
      .then((r) => setEmployees((r.employees ?? []).map((e) => ({ id: e.id, name: `${e.lastName} ${e.firstName}` }))))
      .catch(() => {});
  }, []);

  const handleSave = async () => {
    if (!id) return;
    setSaving(true);
    try {
      // send only changed fields
      const body: any = {};
      if ((issue?.title ?? '') !== editForm.title) body.title = editForm.title || null;
      if (String(issue?.status ?? '') !== editForm.status) body.status = editForm.status !== '' ? Number(editForm.status) as IssueStatus : null;
      if ((issue?.comments ?? '') !== editForm.comments) body.comments = { hasValue: true, value: editForm.comments || null };
      if (String(issue?.priority ?? '') !== editForm.priority) body.priority = editForm.priority ? Number(editForm.priority) : null;
      const originalExecutor = issue?.executor ? String(issue.executor.id) : '';
      if (originalExecutor !== editForm.executorId) body.executorId = editForm.executorId ? Number(editForm.executorId) : null;

      if (Object.keys(body).length === 0) {
        toast('info', 'No changes to save');
        setEditing(false);
        return;
      }

      await issueApi.update(Number(id), body);
      toast('success', 'Issue updated');
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
      await issueApi.delete(Number(id));
      toast('success', 'Issue deleted');
      navigate('/issues');
    } catch (e: any) {
      toast('error', e.message || 'Delete failed');
    } finally {
      setDeleteLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="p-8 animate-pulse space-y-4">
        <div className="h-8 bg-gray-200 rounded w-64" />
        <div className="h-48 bg-gray-200 rounded-2xl" />
      </div>
    );
  }

  if (!issue) {
    return (
      <div className="p-8 text-center">
        <p className="text-gray-400">Issue not found</p>
        <Button variant="ghost" onClick={() => navigate('/issues')} className="mt-4">
          <ArrowLeft size={16} /> Go back
        </Button>
      </div>
    );
  }

  return (
    <div className="p-8">
      <div className="flex items-start gap-4 mb-6">
        <Button variant="ghost" size="sm" onClick={() => navigate('/issues')}>
          <ArrowLeft size={16} /> Back
        </Button>
        <div className="flex-1">
          <h1 className="text-2xl font-bold text-gray-900">{issue.title}</h1>
          <div className="flex items-center gap-2 mt-1">
            <Badge variant={statusVariant(issue.status)}>{IssueStatusLabel[issue.status]}</Badge>
            <Badge variant={priorityVariant(issue.priority)}>{issue.priority}</Badge>
          </div>
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

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Main info */}
        <div className="lg:col-span-2 bg-white rounded-2xl border border-gray-100 shadow-sm p-6">
          <h2 className="text-base font-semibold text-gray-700 mb-4">Issue Details</h2>
          {editing ? (
            <div className="flex flex-col gap-4">
              <Input label="Title" value={editForm.title} onChange={(e) => setEditForm((f) => ({ ...f, title: e.target.value }))} />
              <div className="grid grid-cols-2 gap-3">
                <Select
                  label="Status"
                  value={editForm.status}
                  onChange={(e) => setEditForm((f) => ({ ...f, status: e.target.value }))}
                  options={[
                    { value: IssueStatus.ToDo, label: 'To Do' },
                    { value: IssueStatus.InProgress, label: 'In Progress' },
                    { value: IssueStatus.Done, label: 'Done' },
                  ]}
                />
                <Input label="Priority (1-10)" type="number" min="1" max="10" value={editForm.priority} onChange={(e) => setEditForm((f) => ({ ...f, priority: e.target.value }))} />
              </div>
              <Select
                label="Executor"
                value={editForm.executorId}
                onChange={(e) => setEditForm((f) => ({ ...f, executorId: e.target.value }))}
                placeholder="Select..."
                options={employees.map((e) => ({ value: e.id, label: e.name }))}
              />
              <Textarea label="Comments" value={editForm.comments} onChange={(e) => setEditForm((f) => ({ ...f, comments: e.target.value }))} rows={4} />
            </div>
          ) : (
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-xs text-gray-400 font-medium uppercase tracking-wide mb-1">Status</p>
                  <Badge variant={statusVariant(issue.status)}>{IssueStatusLabel[issue.status]}</Badge>
                </div>
                <div>
                  <p className="text-xs text-gray-400 font-medium uppercase tracking-wide mb-1">Priority</p>
                  <Badge variant={priorityVariant(issue.priority)}>{issue.priority}</Badge>
                </div>
                <div>
                  <p className="text-xs text-gray-400 font-medium uppercase tracking-wide mb-1">Author</p>
                  {issue.author ? (
                    <Link to={`/employees/${issue.author.id}`} className="text-sm font-medium text-indigo-600 hover:text-indigo-700">
                      {issue.author.lastName} {issue.author.firstName}
                    </Link>
                  ) : <p className="text-sm text-gray-400">—</p>}
                </div>
                <div>
                  <p className="text-xs text-gray-400 font-medium uppercase tracking-wide mb-1">Executor</p>
                  {issue.executor ? (
                    <Link to={`/employees/${issue.executor.id}`} className="text-sm font-medium text-indigo-600 hover:text-indigo-700">
                      {issue.executor.lastName} {issue.executor.firstName}
                    </Link>
                  ) : <p className="text-sm text-gray-400">—</p>}
                </div>
              </div>
              {issue.comments && (
                <div>
                  <p className="text-xs text-gray-400 font-medium uppercase tracking-wide mb-2">Comments</p>
                  <p className="text-sm text-gray-700 whitespace-pre-wrap bg-gray-50 rounded-xl p-4">{issue.comments}</p>
                </div>
              )}
            </div>
          )}
        </div>

        {/* Project sidebar */}
        <div className="bg-white rounded-2xl border border-gray-100 shadow-sm p-6">
          <h2 className="text-base font-semibold text-gray-700 mb-4">Project</h2>
          {issue.project ? (
            <div className="space-y-3">
              <Link to={`/projects/${issue.project.id}`} className="block font-medium text-indigo-600 hover:text-indigo-700 text-sm">
                {issue.project.title}
              </Link>
              <div>
                <p className="text-xs text-gray-400 uppercase tracking-wide font-medium mb-1">Customer</p>
                <p className="text-sm text-gray-700">{issue.project.companyCustomer}</p>
              </div>
              <div>
                <p className="text-xs text-gray-400 uppercase tracking-wide font-medium mb-1">Executor</p>
                <p className="text-sm text-gray-700">{issue.project.companyExecuter}</p>
              </div>
              <div>
                <p className="text-xs text-gray-400 uppercase tracking-wide font-medium mb-1">Priority</p>
                <Badge variant={priorityVariant(issue.project.priority)}>{issue.project.priority}</Badge>
              </div>
            </div>
          ) : (
            <p className="text-gray-400 text-sm">No project linked</p>
          )}
        </div>
      </div>

      <ConfirmDialog
        isOpen={deleting}
        onClose={() => setDeleting(false)}
        onConfirm={handleDelete}
        title="Delete Issue"
        message={`Are you sure you want to delete "${issue.title}"?`}
        loading={deleteLoading}
      />
    </div>
  );
};
