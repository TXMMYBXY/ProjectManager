import React, { useEffect, useState, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Search, Trash2, Eye, FolderKanban, ChevronUp, ChevronDown, Calendar } from 'lucide-react';
import { projectApi } from '../../api/client';
import { ProjectItemDto, ProjectQueryParams } from '../../api/types';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Pagination } from '../../components/ui/Pagination';
import { ConfirmDialog } from '../../components/ui/ConfirmDialog';
import { useToast } from '../../components/ui/Toast';
import { CreateProjectModal } from './CreateProjectModal';

const priorityVariant = (p: number): 'danger' | 'warning' | 'success' => {
  if (p >= 8) return 'danger';
  if (p >= 5) return 'warning';
  return 'success';
};

const fmt = (d?: string | null) => {
  if (!d) return '—';
  return new Date(d).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
};

export const ProjectList: React.FC = () => {
  const { toast } = useToast();
  const [projects, setProjects] = useState<ProjectItemDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [loading, setLoading] = useState(false);

  const [filters, setFilters] = useState({
    Title: '',
    CompanyCustomer: '',
    CompanyExecuter: '',
    StartDateFrom: '',
    StartDateTo: '',
    FinishDateFrom: '',
    FinishDateTo: '',
    Priority: '',
  });
  const [sort, setSort] = useState({ field: '', desc: false });
  const [selected, setSelected] = useState<Set<number>>(new Set());
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const [bulkDeleting, setBulkDeleting] = useState(false);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [showCreate, setShowCreate] = useState(false);

  const load = useCallback(async (page = currentPage) => {
    setLoading(true);
    try {
      const params: ProjectQueryParams = {
        PageNumber: page,
        PageSize: pageSize,
        ...Object.fromEntries(Object.entries(filters).filter(([, v]) => v)),
        ...(sort.field && { SortBy: sort.field, Descending: sort.desc }),
      };
      const res = await projectApi.list(params);
      setProjects(res.projects ?? []);
      setTotalCount(res.totalCount);
      setTotalPages(res.totalPages || 1);
      setCurrentPage(res.currentPage || page);
    } catch (e: any) {
      toast('error', e.message || 'Failed to load projects');
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

  const toggleAll = () => {
    setSelected(selected.size === projects.length ? new Set() : new Set(projects.map((p) => p.id)));
  };

  const handleDelete = async () => {
    if (!deletingId) return;
    setDeleteLoading(true);
    try {
      await projectApi.delete(deletingId);
      toast('success', 'Project deleted');
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
      await projectApi.bulkDelete({ ids: Array.from(selected) });
      toast('success', `${selected.size} projects deleted`);
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
          <h1 className="text-2xl font-bold text-gray-900">Projects</h1>
          <p className="text-gray-500 mt-1">{totalCount} total projects</p>
        </div>
        <Button onClick={() => setShowCreate(true)}>
          <Plus size={16} /> New Project
        </Button>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-2xl border border-gray-100 shadow-sm p-4 mb-4">
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
          <input placeholder="Title..." value={filters.Title} onChange={(e) => setFilters(f => ({ ...f, Title: e.target.value }))} className="pl-3 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
          <input placeholder="Customer..." value={filters.CompanyCustomer} onChange={(e) => setFilters(f => ({ ...f, CompanyCustomer: e.target.value }))} className="pl-3 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
          <input placeholder="Executor..." value={filters.CompanyExecuter} onChange={(e) => setFilters(f => ({ ...f, CompanyExecuter: e.target.value }))} className="pl-3 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
          <input type="number" placeholder="Priority..." value={filters.Priority} onChange={(e) => setFilters(f => ({ ...f, Priority: e.target.value }))} className="pl-3 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
          <div className="flex items-center gap-2">
            <label className="text-sm">Start:</label>
            <input type="date" value={filters.StartDateFrom} onChange={(e) => setFilters(f => ({ ...f, StartDateFrom: e.target.value }))} className="pl-3 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            <span>-</span>
            <input type="date" value={filters.StartDateTo} onChange={(e) => setFilters(f => ({ ...f, StartDateTo: e.target.value }))} className="pl-3 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div className="flex items-center gap-2">
            <label className="text-sm">Finish:</label>
            <input type="date" value={filters.FinishDateFrom} onChange={(e) => setFilters(f => ({ ...f, FinishDateFrom: e.target.value }))} className="pl-3 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            <span>-</span>
            <input type="date" value={filters.FinishDateTo} onChange={(e) => setFilters(f => ({ ...f, FinishDateTo: e.target.value }))} className="pl-3 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
          </div>
        </div>
      </div>

      {/* Bulk actions */}
      {selected.size > 0 && (
        <div className="flex items-center gap-3 mb-4 px-4 py-3 bg-indigo-50 border border-indigo-200 rounded-xl">
          <span className="text-sm font-medium text-indigo-700">{selected.size} selected</span>
          <Button variant="danger" size="sm" onClick={() => setBulkDeleting(true)}>
            <Trash2 size={14} /> Delete Selected
          </Button>
          <Button variant="ghost" size="sm" onClick={() => setSelected(new Set())}>Clear</Button>
        </div>
      )}

      {/* Table */}
      <div className="bg-white rounded-2xl border border-gray-100 shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-100">
              <tr>
                <th className={thClass}>
                  <input type="checkbox" checked={selected.size === projects.length && projects.length > 0} onChange={toggleAll} className="rounded border-gray-300" />
                </th>
                {sortableTh('Title', 'Title')}
                {sortableTh('Customer', 'CompanyCustomer')}
                {sortableTh('Executor', 'CompanyExecuter')}
                {sortableTh('Manager', 'ProjectManagerLastName')}
                {sortableTh('Start Date', 'StartDate')}
                {sortableTh('Finish Date', 'FinishDate')}
                {sortableTh('Priority', 'Priority')}
                <th className={thClass}>Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-50">
              {loading ? (
                Array.from({ length: pageSize }).map((_, i) => (
                  <tr key={i} className="animate-pulse">
                    {Array.from({ length: 9 }).map((__, j) => (
                      <td key={j} className="px-4 py-4"><div className="h-4 bg-gray-200 rounded" /></td>
                    ))}
                  </tr>
                ))
              ) : projects.length === 0 ? (
                <tr>
                  <td colSpan={9} className="px-4 py-16 text-center">
                    <FolderKanban size={40} className="text-gray-300 mx-auto mb-3" />
                    <p className="text-gray-400 text-sm">No projects found</p>
                  </td>
                </tr>
              ) : (
                projects.map((p) => (
                  <tr key={p.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-4 py-3">
                      <input type="checkbox" checked={selected.has(p.id)} onChange={() => toggleSelect(p.id)} className="rounded border-gray-300" />
                    </td>
                    <td className="px-4 py-3 font-medium text-gray-800 text-sm max-w-[180px] truncate">{p.title}</td>
                    <td className="px-4 py-3 text-gray-600 text-sm">{p.companyCustomer}</td>
                    <td className="px-4 py-3 text-gray-600 text-sm">{p.companyExecuter}</td>
                    <td className="px-4 py-3 text-gray-500 text-sm">
                      {p.projectManager ? `${p.projectManager.lastName} ${p.projectManager.firstName}` : '—'}
                    </td>
                    <td className="px-4 py-3 text-gray-500 text-sm whitespace-nowrap">
                      <span className="flex items-center gap-1"><Calendar size={12} />{fmt(p.startDate)}</span>
                    </td>
                    <td className="px-4 py-3 text-gray-500 text-sm whitespace-nowrap">{fmt(p.finishDate)}</td>
                    <td className="px-4 py-3">
                      <Badge variant={priorityVariant(p.priority)}>{p.priority}</Badge>
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        <Link to={`/projects/${p.id}`} className="p-1.5 rounded-lg text-gray-400 hover:text-indigo-600 hover:bg-indigo-50 transition-colors">
                          <Eye size={15} />
                        </Link>
                        <button onClick={() => setDeletingId(p.id)} className="p-1.5 rounded-lg text-gray-400 hover:text-red-600 hover:bg-red-50 transition-colors">
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
          <Pagination
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={setCurrentPage}
            totalCount={totalCount}
            pageSize={pageSize}
          />
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

      <CreateProjectModal isOpen={showCreate} onClose={() => setShowCreate(false)} onCreated={() => load(1)} />
      <ConfirmDialog isOpen={!!deletingId} onClose={() => setDeletingId(null)} onConfirm={handleDelete} title="Delete Project" message="Are you sure you want to delete this project?" loading={deleteLoading} />
      <ConfirmDialog isOpen={bulkDeleting} onClose={() => setBulkDeleting(false)} onConfirm={handleBulkDelete} title="Delete Projects" message={`Delete ${selected.size} projects?`} loading={deleteLoading} />
    </div>
  );
};
