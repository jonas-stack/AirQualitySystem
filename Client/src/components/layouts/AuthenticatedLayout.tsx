import { SidebarProvider, SidebarTrigger } from '../ui/sidebar';
import { AppSidebar } from '../sidebar/AppSidebar';
import { Outlet } from 'react-router-dom';
import { Separator } from '../ui/separator';
import { Badge } from '../ui/badge';
import { Button } from '../ui/button';
import DynamicBreadcrumbs from '../header/DynamicBreadcrumbs';
import { SearchForm } from '../header/SearchForm';
import {useEffect, useMemo, useState} from 'react';
import { ThemeToggle } from './ThemeToggle';
import { SearchDialog } from '../command/SearchDialog';
import { LogOut } from 'lucide-react';
import {useSubscriptionHook} from '@/hooks/use-subscribe-to-topic';
import {WebsocketTopics} from "@/generated-client.ts";
import {useAutoSubscription} from "@/hooks/use-auto-subscription.ts";

export const AuthenticatedLayout = () => {
    useSubscriptionHook();
  
    const [commandSearchOpen, setCommandSearchOpen] = useState(false);

    // brug memo for at sikre den ikke bliver oprettet på ny heletiden og kalder flere gange
    const topicIds = useMemo(() => [WebsocketTopics.Dashboard, WebsocketTopics.Ai], []);
    useAutoSubscription(topicIds);

    useEffect(() => {
        const handleKeyDown = (event: KeyboardEvent) => {
          if ((event.ctrlKey || event.metaKey) && event.key === 's') {
            event.preventDefault();
            setCommandSearchOpen(true);
          }
        };
    
        window.addEventListener('keydown', handleKeyDown);
    
        return () => {
          window.removeEventListener('keydown', handleKeyDown);
        };
      }, []);
    

  return (
    <>
    <SidebarProvider>
        <AppSidebar />
        <main className="relative flex w-full flex-1 flex-col bg-background md:peer-data-[variant=inset]:m-2 md:peer-data-[state=collapsed]:peer-data-[variant=inset]:ml-2 md:peer-data-[variant=inset]:ml-0 md:peer-data-[variant=inset]:rounded-md md:peer-data-[variant=inset]:shadow">
          <div className="sticky top-0 z-10 flex flex-col">
            <header className="flex bg-background/70 backdrop-blur-lg border-b h-14 shrink-0 items-center gap-2 transition-[width,height] ease-linear group-has-[[data-collapsible=icon]]/sidebar-wrapper:h-12">
              <div className="flex h-4 items-center gap-2 px-4 w-full">
                <SidebarTrigger />
                <Separator className="mr-1" orientation="vertical" />
                
                <DynamicBreadcrumbs />
                <div className="ml-auto flex items-center gap-2 md:flex-1 md:justify-end">
                  <Badge 
                    variant="default"
                    className="mr-2"
                  >
                    Kasper Mortensen
                  </Badge>
                  
                  <SearchForm setOpen={setCommandSearchOpen} />
                  <div className="flex items-center gap-0.5">
                    <ThemeToggle />
                  </div>
                  <div className='h-5'>
                    <Separator className='h-[20px]' orientation='vertical' />
                  </div>
                  <Button variant='ghost' size='icon'>
                    <LogOut />
                  </Button>
                </div>
              </div>
            </header>
          </div>
          <div className="p-4">
          <Outlet />
          </div>
        </main>
    </SidebarProvider>
    <SearchDialog open={commandSearchOpen} setOpen={setCommandSearchOpen} />
    </>
  );
};