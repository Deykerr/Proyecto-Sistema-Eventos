import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login';
import { RegisterComponent } from './features/auth/register/register';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password'; 
import { ResetPasswordComponent } from './features/auth/reset-password/reset-password';   

import { ProfileComponent } from './features/profile/profile';

import { EventListComponent } from './features/events/event-list/event-list'; 
import { EventFormComponent } from './features/events/event-form/event-form';
import { EventDetailComponent } from './features/events/event-detail/event-detail';

import { MyReservationsComponent } from './features/reservations/my-reservations/my-reservations';

import { adminGuard } from './core/guards/admin-guard';
import { CategoryManagementComponent } from './features/admin/category-management/category-management';
import { DashboardComponent } from './features/dashboard/dashboard';
import { organizerGuard } from './core/guards/organizer-guard';

import { NotificationsComponent } from './features/notifications/notifications';


export const routes: Routes = [
  // Ruta por defecto: Redirigir al Login
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  
  // Ruta de autenticacion
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  
  //perfil de usuario
  { path: 'profile', component: ProfileComponent },

  // Rutas de eventos
  { path: 'events', component: EventListComponent },
  { path: 'events/create', component: EventFormComponent },
  { path: 'events/:id', component: EventDetailComponent },

  //Rutas de Reservacion
  { path: 'my-reservations', component: MyReservationsComponent },

  // Ruta protegida
  { 
    path: 'admin/categories', 
    component: CategoryManagementComponent,
    canActivate: [adminGuard] // <--- AQUÍ APLICAMOS SEGURIDAD
  },

  { 
    path: 'dashboard', 
    component: DashboardComponent,
    canActivate: [organizerGuard] 
  },

  //notificaciones
  { path: 'notifications', component: NotificationsComponent },

];