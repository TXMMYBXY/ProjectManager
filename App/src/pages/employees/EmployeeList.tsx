import React, { useEffect, useState, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Search, Trash2, Eye, UserCircle, ChevronUp, ChevronDown } from 'lucide-react';
import { employeeApi } from '../../api/client';
import { EmployeeItemDto, EmployeeQueryParams } from '../../api/types';
import { Button } from '../../components/ui/Button';
import { Pagination } from '../../components/ui/Pagination';
import { ConfirmDialog } from '../../components/ui/ConfirmDialog';
import { useToast } from '../../components/ui/Toast';
import { CreateEmployeeModal } from './CreateEmployeeModal';

export const EmployeeList: React.FC = () => {
  const { toast } = useToast();
  const [employees, setEmployees] = useState<EmployeeItemDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [loading, setLoading] = useState(false);

  const [filters, setFilters] = useState({ FirstName: '', LastName: '', Patronymic: '', Email: '' });
  const [sort, setSort] = useState({ field: '', desc: false });
  const [selected, setSelected] = useState<Set<number>>(new Set());
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const [bulkDeleting, setBulkDeleting] = useState(false);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [showCreate, setShowCreate] = useState(false);

  const load = useCallback(async (page = currentPage) => {
    setLoading(true);
    try {
      const params: EmployeeQueryParams = {
        PageNumber: page,
        PageSize: pageSize,
        ...Object.fromEntries(Object.entries(filters).filter(([, v]) => v)),
        ...(sort.field && { SortField: sort.field, Descending: sort.desc }),
      };
      const res = await employeeApi.list(params);
      setEmployees(res.employees ?? []);
      setTotalCount(res.totalCount);
      setTotalPages(res.totalPages || 1);
      setCurrentPage(res.currentPage || page);
    } catch (e: any) {
      toast('error', e.message || 'Failed to load employees');
    } finally {
      setLoading(false);
    }
  }, [currentPage, pageSize, filters, sort, toast]);

  useEffect(() => { load(1); }, [pageSize, filters, sort]);
  useEffect(() => { load(currentPage); }, [currentPage]);

  const handleSort = (field: string) => {
    setSort((prev) =>
      prev.field === field ? { field, desc: !prev.desc } : { field, desc: false }
    );
  };

  const toggleSelect = (id: number) => {
    setSelected((prev) => {
      const next = new Set(prev);
      next.has(id) ? next.delete(id) : next.add(id);
      return next;
    });
  };

  const toggleAll = () => {
    if (selected.size === employees.length) setSelected(new Set());
    else setSelected(new Set(employees.map((e) => e.id)));
  };

  const handleDelete = async () => {
    if (!deletingId) return;
    setDeleteLoading(true);
    try {
      await employeeApi.delete(deletingId);
      toast('success', 'Employee deleted');
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
      await employeeApi.bulkDelete({ ids: Array.from(selected) });
      toast('success', `${selected.size} employees deleted`);
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
    if (sort.field !== field) return <span className="w-4 h-4 inline-block" />;
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
          <h1 className="text-2xl font-bold text-gray-900">Employees</h1>
          <p className="text-gray-500 mt-1">{totalCount} total employees</p>
        </div>
        <Button onClick={() => setShowCreate(true)}>
          <Plus size={16} /> Add Employee
        </Button>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-2xl border border-gray-100 shadow-sm p-4 mb-4">
        <div className="grid grid-cols-1 sm:grid-cols-4 gap-3">
          <input placeholder="First name..." value={filters.FirstName} onChange={(e) => setFilters(f => ({ ...f, FirstName: e.target.value }))} className="pl-3 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
          <input placeholder="Last name..." value={filters.LastName} onChange={(e) => setFilters(f => ({ ...f, LastName: e.target.value }))} className="pl-3 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
          <input placeholder="Patronymic..." value={filters.Patronymic} onChange={(e) => setFilters(f => ({ ...f, Patronymic: e.target.value }))} className="pl-3 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
          <input placeholder="Email..." value={filters.Email} onChange={(e) => setFilters(f => ({ ...f, Email: e.target.value }))} className="pl-3 pr-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
        </div>
      </div>

      {/* Bulk actions */}
      {selected.size > 0 && (
        <div className="flex items-center gap-3 mb-4 px-4 py-3 bg-indigo-50 border border-indigo-200 rounded-xl">
          <span className="text-sm font-medium text-indigo-700">{selected.size} selected</span>
          <Button variant="danger" size="sm" onClick={() => setBulkDeleting(true)}>
            <Trash2 size={14} /> Delete Selected
          </Button>
          <Button variant="ghost" size="sm" onClick={() => setSelected(new Set())}>
            Clear
          </Button>
        </div>
      )}

      {/* Table */}
      <div className="bg-white rounded-2xl border border-gray-100 shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-100">
              <tr>
                <th className={thClass}>
                  <input
                    type="checkbox"
                    checked={selected.size === employees.length && employees.length > 0}
                    onChange={toggleAll}
                    className="rounded border-gray-300"
                  />
                </th>
                {sortableTh('Last Name', 'LastName')}
                {sortableTh('First Name', 'FirstName')}
                <th className={thClass}>Patronymic</th>
                {sortableTh('Email', 'Email')}
                <th className={thClass}>Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-50">
              {loading ? (
                Array.from({ length: pageSize }).map((_, i) => (
                  <tr key={i} className="animate-pulse">
                    <td className="px-4 py-4"><div className="w-4 h-4 bg-gray-200 rounded" /></td>
                    <td className="px-4 py-4"><div className="h-4 bg-gray-200 rounded w-24" /></td>
                    <td className="px-4 py-4"><div className="h-4 bg-gray-200 rounded w-20" /></td>
                    <td className="px-4 py-4"><div className="h-4 bg-gray-200 rounded w-16" /></td>
                    <td className="px-4 py-4"><div className="h-4 bg-gray-200 rounded w-32" /></td>
                    <td className="px-4 py-4"><div className="h-4 bg-gray-200 rounded w-16" /></td>
                  </tr>
                ))
              ) : employees.length === 0 ? (
                <tr>
                  <td colSpan={6} className="px-4 py-16 text-center">
                    <UserCircle size={40} className="text-gray-300 mx-auto mb-3" />
                    <p className="text-gray-400 text-sm">No employees found</p>
                  </td>
                </tr>
              ) : (
                employees.map((emp) => (
                  <tr key={emp.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-4 py-3">
                      <input
                        type="checkbox"
                        checked={selected.has(emp.id)}
                        onChange={() => toggleSelect(emp.id)}
                        className="rounded border-gray-300"
                      />
                    </td>
                    <td className="px-4 py-3 font-medium text-gray-800 text-sm">{emp.lastName}</td>
                    <td className="px-4 py-3 text-gray-600 text-sm">{emp.firstName}</td>
                    <td className="px-4 py-3 text-gray-500 text-sm">{emp.patronymic || '—'}</td>
                    <td className="px-4 py-3 text-gray-600 text-sm">{emp.email}</td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        <Link
                          to={`/employees/${emp.id}`}
                          className="p-1.5 rounded-lg text-gray-400 hover:text-indigo-600 hover:bg-indigo-50 transition-colors"
                        >
                          <Eye size={15} />
                        </Link>
                        <button
                          onClick={() => setDeletingId(emp.id)}
                          className="p-1.5 rounded-lg text-gray-400 hover:text-red-600 hover:bg-red-50 transition-colors"
                        >
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

      <CreateEmployeeModal isOpen={showCreate} onClose={() => setShowCreate(false)} onCreated={() => load(1)} />

      <ConfirmDialog
        isOpen={!!deletingId}
        onClose={() => setDeletingId(null)}
        onConfirm={handleDelete}
        title="Delete Employee"
        message="Are you sure you want to delete this employee? This action cannot be undone."
        loading={deleteLoading}
      />

      <ConfirmDialog
        isOpen={bulkDeleting}
        onClose={() => setBulkDeleting(false)}
        onConfirm={handleBulkDelete}
        title="Delete Employees"
        message={`Are you sure you want to delete ${selected.size} employees? This action cannot be undone.`}
        loading={deleteLoading}
      />
    </div>
  );
};
