import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import IndexPage from './pages/IndexPage'

export default function App() {
  return (
      <Routes>
        <Route path="/" element={<IndexPage />} />
      </Routes>
  )
}
