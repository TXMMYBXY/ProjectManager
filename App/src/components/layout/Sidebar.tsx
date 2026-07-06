import React from 'react';
import { NavLink } from 'react-router-dom';
import { Users, FolderKanban, CircleDot, ChevronRight } from 'lucide-react';

const navItems = [
  { to: '/projects', label: 'Projects', icon: FolderKanban, end: false },
  { to: '/employees', label: 'Employees', icon: Users, end: false },
  { to: '/issues', label: 'Issues', icon: CircleDot, end: false },
];

export const Sidebar: React.FC = () => {
  return (
    <aside className="w-64 bg-gray-900 min-h-screen flex flex-col shrink-0">
      {/* Logo */}
      <div className="px-6 py-5 border-b border-gray-800">
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 rounded-lg bg-indigo-500 flex items-center justify-center">
            <FolderKanban size={18} className="text-white" />
          </div>
          <div>
            <p className="text-white font-semibold text-sm">ProjectManager</p>
            <p className="text-gray-400 text-xs">v1.0</p>
          </div>
        </div>
      </div>

      {/* Navigation */}
      <nav className="flex-1 px-3 py-4">
        <p className="px-3 mb-2 text-xs font-semibold text-gray-500 uppercase tracking-wider">Menu</p>
        <ul className="space-y-1">
          {navItems.map(({ to, label, icon: Icon, end }) => (
            <li key={to}>
              <NavLink
                to={to}
                end={end}
                className={({ isActive }) =>
                  `flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-all group
                  ${isActive
                    ? 'bg-indigo-600 text-white'
                    : 'text-gray-400 hover:text-white hover:bg-gray-800'
                  }`
                }
              >
                {({ isActive }) => (
                  <>
                    <Icon size={18} className={isActive ? 'text-white' : 'text-gray-400 group-hover:text-white'} />
                    <span className="flex-1">{label}</span>
                    {isActive && <ChevronRight size={14} className="text-indigo-300" />}
                  </>
                )}
              </NavLink>
            </li>
          ))}
        </ul>
      </nav>
    </aside>
  );
};
