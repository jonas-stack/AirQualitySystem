import {Route, Routes} from "react-router";
import LoginPage from "@/pages/public/LoginPage";
import { AuthenticatedLayout } from "@/components/layouts/AuthenticatedLayout";
import DashboardPage from "@/pages/auth/DashboardPage";
import DataPage from "@/pages/auth/DataPage";
import { ROUTE } from "@/routes-constants";
import { ThemeProvider } from "@/context/ThemeContext";
import { Toaster } from "@/components/ui/sonner";
import AIPage from "@/pages/auth/AIPage.tsx";

export default function ApplicationRoutes() {
    return (
        <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
    <Toaster />
    <Routes>
        <Route path="/" element={<LoginPage />} />

    <Route element={<AuthenticatedLayout />}>
        <Route path={ROUTE.DASHBOARD.INDEX} element={<DashboardPage />} />
        <Route path={ROUTE.DASHBOARD.DATA} element={<DataPage />} />
        <Route path={ROUTE.DASHBOARD.AI} element={<AIPage />} />
    </Route>

    </Routes>
    </ThemeProvider>
)
}