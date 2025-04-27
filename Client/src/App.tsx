import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import LoginPage from '@/pages/public/LoginPage';

export default function App() {
  return (
      <Routes>
        <Route path="/" element={<LoginPage />} />
      </Routes>
  )
}
