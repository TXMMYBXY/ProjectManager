import React, { useEffect, useState, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Search, Trash2, Eye, CircleDot, ChevronUp, ChevronDown } from 'lucide-react';
import { issueApi } from '../../api/client';
import { IssueItemDto, IssueQueryParams, IssueStatus, IssueStatusLabel } from '../../api/types';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Pagination } from '../../components/ui/Pagination';
import { ConfirmDialog } from '../../components/ui/ConfirmDialog';
import { useToast } from '../../components/ui/Toast';
import { CreateIssueModal } from './CreateIssueModal';

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

export const IssueList: React.FC = () => {
  const { toast } = useToast();
  const [issues, setIssues] = useState<IssueItemDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [loading, setLoading] = useState(false);

  const [filters, setFilters] = useState({ Title: '', Status: '', ProjectTitle: '', AuthorFullName: '', ExecutorFullName: '' });
  const [sort, setSort] = useState({ field: '', desc: false });
  const [selected, setSelected] = useState<Set<number>>(new Set());
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const [bulkDeleting, setBulkDeleting] = useState(false);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [showCreate, setShowCreate] = useState(false);

  const load = useCallback(async (page = currentPage) => {
    setLoading(true);
    setSelected(new Set());
    try {
      const params: IssueQueryParams = {
        PageNumber: page,
        PageSize: pageSize,
        ...(filters.Title && { Title: filters.Title }),
        ...(filters.Status !== '' && { Status: filters.Status }),
        ...(filters.ProjectTitle && { ProjectTitle: filters.ProjectTitle }),
        ...(filters.AuthorFullName && { AuthorFullName: filters.AuthorFullName }),
        ...(filters.ExecutorFullName && { ExecutorFullName: filters.ExecutorFullName }),
        ...(sort.field && { SortField: sort.field, Descending: sort.desc }),
      };
      const res = await issueApi.list(params);
      setIssues(res.issues ?? []);
      setTotalCount(res.totalCount);
      setTotalPages(res.totalPages || 1);
      setCurrentPage(res.currentPage || page);
    } catch (e: any) {
      toast('error', e.message || 'Failed to load issues');
    } finally {
      setLoading(false);
    }
  }, [currentPage, pageSize, filters, sort, toast]);

  useEffect(() => { load(1); }, [pageSize, filters, sort]);
  useEffect(() => { load(currentPage); }, [currentPage]);

  const handleSort = (field: string) => {
    setSort((prev) => prev.field === field ? { field, desc: !prev.desc } : { field, desc: false });
  };

  const toggleSelect = (id: number) => {
    setSelected((prev) => { const n = new Set(prev); n.has(id) ? n.delete(id) : n.add(id); return n; });
  };
  const toggleAll = () => setSelected(selected.size === issues.length ? new Set() : new Set(issues.map((i) => i.id)));

  const handleDelete = async () => {
    if (!deletingId) return;
    setDeleteLoading(true);
    try {
      await issueApi.delete(deletingId);
      toast('success', 'Issue deleted');
      setDeletingId(null);
      load(currentPage);
    } catch (e: any) {
      toast('error', e.message || 'Delete failed');
    } finally {
      setDeleteLoading(false);
    }
  };

  const handleBulkDelete = async () => {
    setDeleteLoading(true);
    try {
      await issueApi.bulkDelete({ ids: Array.from(selected) });
      toast('success', `${selected.size} issues deleted`);
      setSelected(new Set());
      setBulkDeleting(false);
      load(currentPage);
    } catch (e: any) {
      toast('error', e.message || 'Bulk delete failed');
    } finally {
      setDeleteLoading(false);
    }
  };

  const SortIcon = ({ field }: { field: string }) => {
    if (sort.field !== field) return null;
    return sort.desc ? <ChevronDown size={14} /> : <ChevronUp size={14} />;
  };

  const thClass = 'px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider';
  const sortableTh = (label: string, field: string) => (
    <th className={`${thClass} cursor-pointer hover:text-gray-700 select-none`} onClick={() => handleSort(field)}>
      <span className="flex items-center gap-1">{label} <SortIcon field={field} /></span>
    </th>
  );

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Issues</h1>
          <p className="text-gray-500 mt-1">{totalCount} total issues</p>
        </div>
        <Button onClick={() => setShowCreate(true)}>
          <Plus size={16} /> New Issue
        </Button>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-2xl border border-gray-100 shadow-sm p-4 mb-4">
        <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-3">
          <div className="relative">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              placeholder="Title..."
              value={filters.Title}
              onChange={(e) => setFilters((f) => ({ ...f, Title: e.target.value }))}
              className="w-full pl-9 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
          <select
            value={filters.Status}
            onChange={(e) => setFilters((f) => ({ ...f, Status: e.target.value }))}
            className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 bg-white"
          >
            <option value="">All statuses</option>
            <option value="0">To Do</option>
            <option value="1">In Progress</option>
            <option value="2">Done</option>
          </select>
          <div className="relative">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              placeholder="Project..."
              value={filters.ProjectTitle}
              onChange={(e) => setFilters((f) => ({ ...f, ProjectTitle: e.target.value }))}
              className="w-full pl-9 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
          <div className="relative">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              placeholder="Author..."
              value={filters.AuthorFullName}
              onChange={(e) => setFilters((f) => ({ ...f, AuthorFullName: e.target.value }))}
              className="w-full pl-9 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
          <div className="relative">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              placeholder="Executor..."
              value={filters.ExecutorFullName}
              onChange={(e) => setFilters((f) => ({ ...f, ExecutorFullName: e.target.value }))}
              className="w-full pl-9 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
        </div>
      </div>

      {selected.size > 0 && (
        <div className="flex items-center gap-3 mb-4 px-4 py-3 bg-indigo-50 border border-indigo-200 rounded-xl">
          <span className="text-sm font-medium text-indigo-700">{selected.size} selected</span>
          <Button variant="danger" size="sm" onClick={() => setBulkDeleting(true)}>
            <Trash2 size={14} /> Delete Selected
          </Button>
          <Button variant="ghost" size="sm" onClick={() => setSelected(new Set())}>Clear</Button>
        </div>
      )}

      <div className="bg-white rounded-2xl border border-gray-100 shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-100">
              <tr>
                <th className={thClass}>
                  <input type="checkbox" checked={selected.size === issues.length && issues.length > 0} onChange={toggleAll} className="rounded border-gray-300" />
                </th>
                {sortableTh('Title', 'Title')}
                {sortableTh('Project', 'ProjectTitle')}
                {sortableTh('Author', 'AuthorLastName')}
                {sortableTh('Executor', 'ExecutorLastName')}
                {sortableTh('Status', 'Status')}
                {sortableTh('Priority', 'Priority')}
                <th className={thClass}>Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-50">
              {loading ? (
                Array.from({ length: pageSize }).map((_, i) => (
                  <tr key={i} className="animate-pulse">
                    {Array.from({ length: 8 }).map((__, j) => (
                      <td key={j} className="px-4 py-4"><div className="h-4 bg-gray-200 rounded" /></td>
                    ))}
                  </tr>
                ))
              ) : issues.length === 0 ? (
                <tr>
                  <td colSpan={8} className="px-4 py-16 text-center">
                    <CircleDot size={40} className="text-gray-300 mx-auto mb-3" />
                    <p className="text-gray-400 text-sm">No issues found</p>
                  </td>
                </tr>
              ) : (
                issues.map((issue) => (
                  <tr key={issue.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-4 py-3">
                      <input type="checkbox" checked={selected.has(issue.id)} onChange={() => toggleSelect(issue.id)} className="rounded border-gray-300" />
                    </td>
                    <td className="px-4 py-3 font-medium text-gray-800 text-sm max-w-[200px] truncate">{issue.title}</td>
                    <td className="px-4 py-3 text-gray-500 text-sm">
                      {issue.project
                        ? <Link to={`/projects/${issue.project.id}`} className="hover:text-indigo-600 transition-colors">{issue.project.title}</Link>
                        : '—'}
                    </td>
                    <td className="px-4 py-3 text-gray-600 text-sm">
                      {issue.author
                        ? <Link to={`/employees/${issue.author.id}`} className="hover:text-indigo-600 transition-colors">{issue.author.lastName} {issue.author.firstName}</Link>
                        : '—'}
                    </td>
                    <td className="px-4 py-3 text-gray-600 text-sm">
                      {issue.executor
                        ? <Link to={`/employees/${issue.executor.id}`} className="hover:text-indigo-600 transition-colors">{issue.executor.lastName} {issue.executor.firstName}</Link>
                        : '—'}
                    </td>
                    <td className="px-4 py-3">
                      <Badge variant={statusVariant(issue.status)}>
                        {IssueStatusLabel[issue.status]}
                      </Badge>
                    </td>
                    <td className="px-4 py-3">
                      <Badge variant={priorityVariant(issue.priority)}>{issue.priority}</Badge>
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        <Link to={`/issues/${issue.id}`} className="p-1.5 rounded-lg text-gray-400 hover:text-indigo-600 hover:bg-indigo-50 transition-colors">
                          <Eye size={15} />
                        </Link>
                        <button onClick={() => setDeletingId(issue.id)} className="p-1.5 rounded-lg text-gray-400 hover:text-red-600 hover:bg-red-50 transition-colors">
                          <Trash2 size={15} />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
        <div className="px-4 py-3 border-t border-gray-100 flex justify-between items-center">
          <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={setCurrentPage} totalCount={totalCount} pageSize={pageSize} />
          <div className="flex items-center gap-2">
            <label htmlFor="pageSize" className="text-sm">Page size:</label>
            <select id="pageSize" value={pageSize} onChange={e => setPageSize(Number(e.target.value))} className="px-2 py-1 text-sm border border-gray-300 rounded-lg">
              <option value={10}>10</option>
              <option value={20}>20</option>
              <option value={50}>50</option>
            </select>
          </div>
        </div>
      </div>

      <CreateIssueModal isOpen={showCreate} onClose={() => setShowCreate(false)} onCreated={() => load(1)} />
      <ConfirmDialog isOpen={!!deletingId} onClose={() => setDeletingId(null)} onConfirm={handleDelete} title="Delete Issue" message="Are you sure you want to delete this issue?" loading={deleteLoading} />
      <ConfirmDialog isOpen={bulkDeleting} onClose={() => setBulkDeleting(false)} onConfirm={handleBulkDelete} title="Delete Issues" message={`Delete ${selected.size} issues?`} loading={deleteLoading} />
    </div>
  );
};
