import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import LoginPage from '@/pages/public/LoginPage';
import { AuthenticatedLayout } from './components/layouts/AuthenticatedLayout';
import { ROUTE } from './routes';
import DashboardPage from './pages/auth/DashboardPage';
import { ThemeProvider } from './context/ThemeContext';

export default function App() {
  return (
      <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
        <Routes>
          <Route path="/" element={<LoginPage />} />

          <Route element={<AuthenticatedLayout />}>
            <Route path={ROUTE.DASHBOARD.INDEX} element={<DashboardPage />} />
          </Route>

        </Routes>
      </ThemeProvider>
  )
}
