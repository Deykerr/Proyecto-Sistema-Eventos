// 1. Importa ChangeDetectorRef
import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { EventService } from '../../../services/event.service';
import { Event } from '../../../core/models/event.model';
import { RouterLink } from '@angular/router';
import { CategoryService } from '../../../services/category.service';
import { Category } from '../../../core/models/category.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './event-list.html',
  styleUrl: './event-list.css'
})
export class EventListComponent implements OnInit {
  private eventService = inject(EventService);
  private cd = inject(ChangeDetectorRef); 
  private categoryService = inject(CategoryService);

  allEvents: Event[] = []; // Copia de respaldo de todos los eventos
  events: Event[] = [];    // Lista que se muestra en pantalla
  categories: Category[] = [];
  selectedCategory: string = ''; // Para el select

  isLoading = true;

  ngOnInit(): void {
    console.log('Iniciando carga de eventos...'); // Log de debug
    this.loadEvents();
    this.loadData();
  }

  loadEvents() {
    this.eventService.getAllEvents().subscribe({
      next: (data) => {
        console.log('Eventos recibidos:', data); // Log de éxito
        this.events = data;
        this.isLoading = false;
        this.cd.detectChanges(); // 3. ¡Fuerza la actualización de la vista!
      },
      error: (err) => {
        console.error('Error cargando eventos:', err); // Log de error
        this.isLoading = false;
        this.cd.detectChanges(); // 3. ¡Fuerza la actualización de la vista!
      }
    });
  }

  loadData() {
    // 1. Cargar Categorías
    this.categoryService.getAll().subscribe(cats => this.categories = cats);

    // 2. Cargar Eventos
    this.eventService.getAllEvents().subscribe({
      next: (data) => {
        this.allEvents = data; // Guardamos todo
        this.events = data;    // Inicialmente mostramos todo
        this.isLoading = false;
        this.cd.detectChanges();
      },
      error: () => { this.isLoading = false; }
    });
  }

  // Método para filtrar cuando cambia el select
  filterEvents() {
    if (!this.selectedCategory) {
      this.events = this.allEvents; // Si selecciona "Todas", restauramos
    } else {
      // Filtramos por el nombre de la categoría (que viene en el DTO del evento)
      // OJO: Tu DTO devuelve 'categoryName'. Compara el nombre o el ID si lo tuvieras.
      // Como el select tendrá IDs, idealmente tu Evento debería tener categoryId.
      // Si tu evento solo tiene categoryName, usaremos el texto del select.
      
      // Opción A: Si tu Evento DTO tiene CategoryId (Recomendado, revisa tu modelo)
      // this.events = this.allEvents.filter(e => e.categoryId === this.selectedCategory);

      // Opción B: Filtrado por coincidencia de nombre (Si no tienes ID en el DTO de lista)
      const catName = this.categories.find(c => c.id === this.selectedCategory)?.name;
      if (catName) {
        this.events = this.allEvents.filter(e => e.categoryName === catName);
      }
    }
  }
}