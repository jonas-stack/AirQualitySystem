import { SidebarProvider } from '../ui/sidebar';
import { AppSidebar } from '../sidebar/AppSidebar';
import { Outlet } from 'react-router-dom';

export const AuthenticatedLayout = () => {
  return (
    <SidebarProvider>
      <div className="flex">
        <AppSidebar />
        
        <div className="flex-1">
          
          <div className="p-6">
            <Outlet />
          </div>
        </div>
      </div>
    </SidebarProvider>
  );
};