import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import Swal from 'sweetalert2';
import { EventService } from '../../../services/event.service';
import { CategoryService } from '../../../services/category.service';
import { CreateEventRequest, UpdateEventRequest } from '../../../core/models/event.model';
import { Category } from '../../../core/models/category.model';

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
  private route = inject(ActivatedRoute);

  eventForm: FormGroup = this.fb.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    categoryId: ['', Validators.required],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    location: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0)]],
    capacity: [10, [Validators.required, Validators.min(1)]],
    isPublic: [true]
  });

  categories: Category[] = [];
  isLoading = false;
  isEditMode = false;
  eventId: string | null = null;
  
  // Guardamos el evento original para mantener coordenadas y status al editar
  private originalEvent: any = null;

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('id');

    // 1. PRIMERO cargamos categorías
    this.categoryService.getAll().subscribe({
      next: (cats) => {
        this.categories = cats;

        // 2. SOLO DESPUÉS de tener categorías, cargamos el evento si es edición
        if (this.eventId) {
          this.isEditMode = true;
          this.loadEventData(this.eventId);
        }
      },
      error: (err) => console.error('Error cargando categorías', err)
    });
  }

  loadEventData(id: string) {
    this.isLoading = true;
    this.eventService.getEventById(id).subscribe({
      next: (event) => {
        this.originalEvent = event; // Guardamos respaldo
        
        // Ajuste de fechas para el input datetime-local (YYYY-MM-DDTHH:mm)
        // Cortamos los segundos y milisegundos si vienen
        const start = event.startDate.toString().slice(0, 16);
        const end = event.endDate.toString().slice(0, 16);

        this.eventForm.patchValue({
          title: event.title,
          description: event.description,
          // AHORA USAMOS EL ID DIRECTO (Gracias al cambio en el Backend)
          // Si no hiciste el cambio en el back, usa: this.categories.find(c => c.name === event.categoryName)?.id
          categoryId: (event as any).categoryId || this.categories.find(c => c.name === event.categoryName)?.id, 
          startDate: start,
          endDate: end,
          location: event.location,
          price: event.price,
          capacity: event.capacity,
          isPublic: event.isPublic
        });
        this.isLoading = false;
      },
      error: () => {
        Swal.fire('Error', 'No se pudo cargar el evento', 'error');
        this.router.navigate(['/events/my-events']);
      }
    });
  }

  onSubmit() {
    if (this.eventForm.invalid) {
      this.eventForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const formValue = this.eventForm.value;

    if (this.isEditMode && this.eventId) {
      // --- MODO EDICIÓN ---
      
      // Mapear el status de string a número si es necesario
      // Draft = 0, Published = 1, Canceled = 2
      let statusInt = 0; 
      if (this.originalEvent) {
        if (this.originalEvent.status === 'Published') statusInt = 1;
        else if (this.originalEvent.status === 'Canceled') statusInt = 2;
      }

      const updateRequest: UpdateEventRequest = {
        ...formValue,
        // Mantenemos las coordenadas originales para no perderlas (si no han cambiado la ubicación)
        latitude: this.originalEvent?.latitude || 0,
        longitude: this.originalEvent?.longitude || 0,
        // Enviamos el status como número (más seguro para .NET)
        status: statusInt 
      };

      this.eventService.updateEvent(this.eventId, updateRequest).subscribe({
        next: () => {
          this.isLoading = false;
          Swal.fire('Actualizado', 'Evento modificado correctamente', 'success');
          this.router.navigate(['/events/my-events']);
        },
        error: (err) => {
          this.isLoading = false;
          console.error(err);
          // Muestra el error detallado si el backend lo envía
          const msg = err.error?.message || err.error?.title || 'No se pudo actualizar';
          Swal.fire('Error', `Error al guardar: ${msg}`, 'error');
        }
      });

    } else {
      // --- MODO CREACIÓN ---
      const createRequest: CreateEventRequest = {
        ...formValue,
        latitude: 0,
        longitude: 0
      };

      this.eventService.createEvent(createRequest).subscribe({
        next: () => {
          this.isLoading = false;
          Swal.fire('Creado', 'Evento creado exitosamente', 'success');
          this.router.navigate(['/events/my-events']);
        },
        error: (err) => {
          this.isLoading = false;
          Swal.fire('Error', 'No se pudo crear el evento', 'error');
        }
      });
    }
  }
}