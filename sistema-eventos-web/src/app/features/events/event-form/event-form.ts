import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import Swal from 'sweetalert2';
import { EventService } from '../../../services/event.service';
import { CategoryService } from '../../../services/category.service';
import { Category } from '../../../core/models/category.model';
import { CreateEventRequest } from '../../../core/models/event.model';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './event-form.html',
  styleUrl: './event-form.css'
})
export class EventFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private eventService = inject(EventService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  categories: Category[] = [];

  eventForm: FormGroup = this.fb.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    categoryId: ['', Validators.required], // Aquí pegaremos un GUID válido por ahora
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    location: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0)]],
    capacity: [10, [Validators.required, Validators.min(1)]],
    isPublic: [true]
  });

  isLoading = false;

  // Cargar las categorías al iniciar
  ngOnInit(): void {
    this.categoryService.getAll().subscribe({
      next: (data) => {
        this.categories = data;
      },
      error: (err) => {
        console.error('Error cargando categorías', err);
        // Opcional: Swal.fire('Error', 'No se pudieron cargar las categorías', 'error');
      }
    });
  }

  onSubmit() {
    if (this.eventForm.invalid) {
      this.eventForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    
    // Mapeo de datos: Convertir fechas string a objetos Date si es necesario
    // Angular suele manejar los inputs de fecha como strings "YYYY-MM-DDTHH:mm"
    const formValue = this.eventForm.value;
    
    const request: CreateEventRequest = {
      ...formValue,
      latitude: 0, // Por ahora 0, el backend buscará la ubicación
      longitude: 0
    };

    this.eventService.createEvent(request).subscribe({
      next: () => {
        this.isLoading = false;
        Swal.fire('Creado', 'El evento se ha publicado correctamente', 'success');
        this.router.navigate(['/events']);
      },
      error: (err) => {
        this.isLoading = false;
        console.error(err);
        Swal.fire('Error', 'No se pudo crear el evento', 'error');
      }
    });
  }
}