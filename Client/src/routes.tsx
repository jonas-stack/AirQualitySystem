import {Navigate, Route, Routes} from "react-router";
import { AuthenticatedLayout } from "@/components/layouts/AuthenticatedLayout";
import DashboardPage from "@/pages/auth/DashboardPage";
import DataPage from "@/pages/auth/DataPage";
import { ROUTE } from "@/routes-constants";
import { ThemeProvider } from "@/context/ThemeContext";
import { Toaster } from "@/components/ui/sonner";
import AIPage from "@/pages/auth/AIPage.tsx";
import DevicePage from "./pages/auth/DevicePage";
import { TooltipProvider } from "./components/ui/tooltip";

export default function ApplicationRoutes() {
  return (
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
      <TooltipProvider>
      <Toaster />
      <Routes>
        <Route element={<AuthenticatedLayout />}>
          <Route path="/" element={<Navigate to={ROUTE.DASHBOARD.INDEX} replace />} />

          <Route path={ROUTE.DASHBOARD.INDEX} element={<DashboardPage />} />
          <Route path={ROUTE.DASHBOARD.DATA} element={<DataPage />} />
          <Route path={ROUTE.DASHBOARD.AI} element={<AIPage />} />
          <Route path={ROUTE.DASHBOARD.DEVICE.INDEX} element={<DevicePage />} />
    </Route>

      </Routes>
      </TooltipProvider>
    </ThemeProvider>
  )
}