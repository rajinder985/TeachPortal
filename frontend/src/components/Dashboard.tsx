import React, { useState, useEffect } from 'react';
import {
  Container,
  Paper,
  Typography,
  Box,
  Card,
  CardContent,
  CircularProgress,
  Alert,
  Button,
  Stack,
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Teacher, Student } from '../types';
import apiService from '../services/api';

const Dashboard: React.FC = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [teachers, setTeachers] = useState<Teacher[]>([]);
  const [recentStudents, setRecentStudents] = useState<Student[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      
      // Load teachers and recent students in parallel
      const [teachersData, studentsData] = await Promise.all([
        apiService.getAllTeachers(),
        apiService.getMyStudents(1, 5) // Get first 5 students
      ]);

      setTeachers(teachersData);
      setRecentStudents(studentsData.items);
    } catch (err) {
      setError('Failed to load dashboard data');
      console.error('Dashboard error:', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Container>
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
          <CircularProgress size={60} />
        </Box>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Box mb={4}>
        <Typography variant="h4" gutterBottom>
          Welcome back, {user?.firstName}!
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Here's your teaching portal overview
        </Typography>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}

      {/* Stats Cards */}
      <Stack 
        direction={{ xs: 'column', sm: 'row' }} 
        spacing={3} 
        sx={{ mb: 4 }}
      >
        <Card sx={{ flex: 1 }}>
          <CardContent>
            <Typography color="textSecondary" gutterBottom>
              My Students
            </Typography>
            <Typography variant="h4">
              {user?.studentCount || 0}
            </Typography>
          </CardContent>
        </Card>
        
        <Card sx={{ flex: 1 }}>
          <CardContent>
            <Typography color="textSecondary" gutterBottom>
              Other Teachers
            </Typography>
            <Typography variant="h4">
              {teachers.filter(t => t.id !== user?.id).length}
            </Typography>
          </CardContent>
        </Card>
        
        <Card sx={{ flex: 1 }}>
          <CardContent>
            <Typography color="textSecondary" gutterBottom>
              Total Students
            </Typography>
            <Typography variant="h4">
              {teachers.reduce((sum, t) => sum + t.studentCount, 0)}
            </Typography>
          </CardContent>
        </Card>
      </Stack>

      <Stack direction={{ xs: 'column', md: 'row' }} spacing={3}>
        {/* Recent Students */}
        <Paper sx={{ p: 3, flex: 1 }}>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
            <Typography variant="h6">Recent Students</Typography>
            <Button 
              variant="outlined" 
              size="small"
              onClick={() => navigate('/students/create')}
            >
              Add Student
            </Button>
          </Box>

          {recentStudents.length === 0 ? (
            <Box textAlign="center" py={3}>
              <Typography color="text.secondary">
                No students yet. Create your first student!
              </Typography>
              <Button
                variant="contained"
                sx={{ mt: 2 }}
                onClick={() => navigate('/students/create')}
              >
                Add Student
              </Button>
            </Box>
          ) : (
            <>
              {recentStudents.map((student) => (
                <Box
                  key={student.id}
                  sx={{
                    py: 1.5,
                    borderBottom: '1px solid #e0e0e0',
                    '&:last-child': { borderBottom: 'none' },
                  }}
                >
                  <Typography variant="subtitle1">{student.fullName}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {student.email}
                  </Typography>
                </Box>
              ))}
              <Box mt={2} textAlign="center">
                <Button onClick={() => navigate('/students')}>
                  View All Students
                </Button>
              </Box>
            </>
          )}
        </Paper>

        {/* Other Teachers */}
        <Paper sx={{ p: 3, flex: 1 }}>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
            <Typography variant="h6">Other Teachers</Typography>
            <Button 
              variant="outlined" 
              size="small"
              onClick={() => navigate('/teachers')}
            >
              View All
            </Button>
          </Box>

          {teachers.filter(t => t.id !== user?.id).length === 0 ? (
            <Box textAlign="center" py={3}>
              <Typography color="text.secondary">
                No other teachers in the system yet.
              </Typography>
            </Box>
          ) : (
            teachers
              .filter(t => t.id !== user?.id)
              .slice(0, 5)
              .map((teacher) => (
                <Box
                  key={teacher.id}
                  sx={{
                    py: 1.5,
                    borderBottom: '1px solid #e0e0e0',
                    '&:last-child': { borderBottom: 'none' },
                  }}
                >
                  <Typography variant="subtitle1">{teacher.fullName}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {teacher.studentCount} students
                  </Typography>
                </Box>
              ))
          )}
        </Paper>
      </Stack>
    </Container>
  );
};

export default Dashboard;