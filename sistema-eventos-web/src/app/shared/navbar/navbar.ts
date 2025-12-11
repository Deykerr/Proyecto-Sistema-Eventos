import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class NavbarComponent implements OnInit {
  authService = inject(AuthService);
  router = inject(Router);

  isLoggedIn = false;
  userName = '';
  userRole = '';

  ngOnInit(): void {
    // Nos suscribimos al observable del token para detectar cambios en tiempo real
    this.authService.userToken$.subscribe(token => {
      this.isLoggedIn = !!token;
      if (this.isLoggedIn) {
        this.userRole = this.authService.getUserRole();
        // Podrías decodificar el nombre del token si viene ahí, o dejarlo genérico
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}