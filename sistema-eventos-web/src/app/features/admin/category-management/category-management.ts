import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoryService } from '../../../services/category.service';
import { Category } from '../../../core/models/category.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-category-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './category-management.html',
  styleUrl: './category-management.css'
})
export class CategoryManagementComponent implements OnInit {
  private categoryService = inject(CategoryService);
  private fb = inject(FormBuilder);

  categories: Category[] = [];
  form: FormGroup = this.fb.group({
    name: ['', Validators.required],
    description: ['']
  });

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService.getAll().subscribe(data => this.categories = data);
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.categoryService.create(this.form.value).subscribe({
      next: (newCat) => {
        Swal.fire('Creada', `Categoría ${newCat.name} agregada`, 'success');
        this.categories.push(newCat); // Actualizar tabla sin recargar
        this.form.reset();
      },
      error: (err) => Swal.fire('Error', 'No se pudo crear (¿Nombre duplicado?)', 'error')
    });
  }

  deleteCategory(id: string) {
    Swal.fire({
      title: '¿Eliminar?',
      text: "Esto podría afectar eventos asociados.",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      confirmButtonText: 'Sí, borrar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.categoryService.delete(id).subscribe({
          next: () => {
            this.categories = this.categories.filter(c => c.id !== id);
            Swal.fire('Eliminado', 'Categoría eliminada.', 'success');
          },
          error: () => Swal.fire('Error', 'No se pudo eliminar.', 'error')
        });
      }
    });
  }
}