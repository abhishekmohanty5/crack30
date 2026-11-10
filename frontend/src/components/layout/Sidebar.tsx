import React from 'react';
import { NavLink } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import {
  LayoutDashboard,
  BookOpen,
  Bookmark,
  Search,
  PlusCircle,
  ShieldCheck,
  TrendingUp,
  X,
  Calendar
} from 'lucide-react';

interface SidebarProps {
  isOpen: boolean;
  onClose: () => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ isOpen, onClose }) => {
  const { user } = useAuth();

  const navItems = [
    { label: 'Dashboard', to: '/', icon: LayoutDashboard },
    { label: 'My Progress', to: '/progress', icon: TrendingUp },
    { label: 'Search Content', to: '/search', icon: Search },
    { label: 'Bookmarks', to: '/bookmarks', icon: Bookmark },
    { label: 'Contribute', to: '/contribute', icon: PlusCircle },
  ];

  if (user?.role === 'Admin') {
    navItems.push({ label: 'Admin Panel', to: '/admin', icon: ShieldCheck });
  }

  const linkClass = ({ isActive }: { isActive: boolean }) =>
    `flex items-center gap-3 px-3.5 py-2.5 rounded-xl text-sm font-medium transition-all ${
      isActive
        ? 'bg-indigo-600 text-white shadow-md shadow-indigo-600/20'
        : 'text-slate-600 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800/60 hover:text-slate-900 dark:hover:text-slate-100'
    }`;

  return (
    <>
      {isOpen && (
        <div
          onClick={onClose}
          className="fixed inset-0 z-40 bg-slate-950/40 backdrop-blur-sm md:hidden"
        />
      )}

      <aside
        className={`fixed top-0 bottom-0 left-0 z-50 w-64 border-r border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 transition-transform duration-300 ease-in-out md:translate-x-0 md:static ${
          isOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        <div className="flex flex-col h-full">
          <div className="flex h-16 items-center justify-between px-6 border-b border-slate-100 dark:border-slate-800/80 md:hidden">
            <span className="font-bold text-lg text-slate-800 dark:text-slate-100">Menu</span>
            <button
              onClick={onClose}
              className="p-1.5 rounded-lg text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800"
            >
              <X className="w-5 h-5" />
            </button>
          </div>

          <div className="p-4 space-y-1 flex-1 overflow-y-auto">
            <div className="px-3 py-2 text-xs font-semibold text-slate-400 uppercase tracking-wider">
              Navigation
            </div>
            {navItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                onClick={() => onClose()}
                className={linkClass}
              >
                <item.icon className="w-4 h-4 shrink-0" />
                <span>{item.label}</span>
              </NavLink>
            ))}
          </div>

          {/* Hiring Timeline Quick Info */}
          <div className="p-4 m-3 rounded-2xl bg-gradient-to-br from-indigo-50 to-blue-50 dark:from-indigo-950/40 dark:to-blue-950/40 border border-indigo-100 dark:border-indigo-900/50">
            <div className="flex items-center gap-2 text-indigo-700 dark:text-indigo-400 text-xs font-semibold uppercase tracking-wider mb-1">
              <Calendar className="w-3.5 h-3.5" />
              Target Milestone
            </div>
            <p className="text-xs font-semibold text-slate-800 dark:text-slate-200">
              Technical Assessment
            </p>
            <p className="text-[11px] text-slate-500 dark:text-slate-400 mt-0.5">
              SHL Platform at Mouser Premise
            </p>
          </div>
        </div>
      </aside>
    </>
  );
};
