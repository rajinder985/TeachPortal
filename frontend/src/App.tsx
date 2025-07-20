import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate, useNavigate, useLocation } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { 
  CssBaseline, 
  AppBar, 
  Toolbar, 
  Typography, 
  Button, 
  Box,
  IconButton,
  Stack
} from '@mui/material';
import { Home, Dashboard as DashboardIcon, School, People } from '@mui/icons-material';
import { AuthProvider, useAuth } from './context/AuthContext';
import Login from './components/Login';
import Register from './components/Register';
import Dashboard from './components/Dashboard';
import StudentList from './components/StudentList';
import CreateStudent from './components/CreateStudent';
import TeacherList from './components/TeacherList';

const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
  },
});


const Navigation: React.FC = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  if (!user) return null;

  const isActive = (path: string) => location.pathname === path;

  return (
    <AppBar position="static">
      <Toolbar>
        <Typography variant="h6" component="div" sx={{ mr: 3 }}>
          Teacher Portal
        </Typography>
        
        {/* Navigation Buttons */}
        <Stack direction="row" spacing={1} sx={{ flexGrow: 1 }}>
          <Button
            color="inherit"
            startIcon={<Home />}
            onClick={() => navigate('/dashboard')}
            variant={isActive('/dashboard') ? 'outlined' : 'text'}
            sx={{ 
              color: 'white',
              borderColor: isActive('/dashboard') ? 'white' : 'transparent',
            }}
          >
            Dashboard
          </Button>
          
          <Button
            color="inherit"
            startIcon={<School />}
            onClick={() => navigate('/students')}
            variant={isActive('/students') || isActive('/students/create') ? 'outlined' : 'text'}
            sx={{ 
              color: 'white',
              borderColor: (isActive('/students') || isActive('/students/create')) ? 'white' : 'transparent',
            }}
          >
            My Students
          </Button>
          
          <Button
            color="inherit"
            startIcon={<People />}
            onClick={() => navigate('/teachers')}
            variant={isActive('/teachers') ? 'outlined' : 'text'}
            sx={{ 
              color: 'white',
              borderColor: isActive('/teachers') ? 'white' : 'transparent',
            }}
          >
            Teachers
          </Button>
        </Stack>

        {/* User Info and Logout */}
        <Stack direction="row" alignItems="center" spacing={2}>
          <Typography variant="body2">
            Welcome, {user.firstName}
          </Typography>
          <Button color="inherit" onClick={logout} variant="outlined">
            Logout
          </Button>
        </Stack>
      </Toolbar>
    </AppBar>
  );
};

// Protected Route component
const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated, loading } = useAuth();

  if (loading) {
    return (
      <Box 
        display="flex" 
        justifyContent="center" 
        alignItems="center" 
        minHeight="100vh"
      >
        Loading...
      </Box>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return (
    <>
      <Navigation />
      <Box sx={{ minHeight: 'calc(100vh - 64px)' }}>
        {children}
      </Box>
    </>
  );
};

// Main App component
const AppContent: React.FC = () => {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route
        path="/dashboard"
        element={
          <ProtectedRoute>
            <Dashboard />
          </ProtectedRoute>
        }
      />
      <Route
        path="/students"
        element={
          <ProtectedRoute>
            <StudentList />
          </ProtectedRoute>
        }
      />
      <Route
        path="/students/create"
        element={
          <ProtectedRoute>
            <CreateStudent />
          </ProtectedRoute>
        }
      />
      <Route
        path="/teachers"
        element={
          <ProtectedRoute>
            <TeacherList />
          </ProtectedRoute>
        }
      />
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      <Route path="*" element={
        <Box 
          display="flex" 
          justifyContent="center" 
          alignItems="center" 
          minHeight="100vh"
        >
          <Typography variant="h4">Page Not Found</Typography>
        </Box>
      } />
    </Routes>
  );
};

const App: React.FC = () => {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Router>
        <AuthProvider>
          <Box sx={{ flexGrow: 1 }}>
            <AppContent />
          </Box>
        </AuthProvider>
      </Router>
    </ThemeProvider>
  );
};

export default App;