import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { ProductService } from '../services/product.service';
import { Product } from '../interfaces/product';
import { FileHandler } from '../interfaces/FileHandler';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-add-product-page',
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './add-product-page.component.html',
  styleUrl: './add-product-page.component.css'
})

export class AddProductPageComponent {
  constructor(private productService: ProductService) { }

  images: FileHandler[] = [];
  thumbnail: FileHandler | null = null;
  
  addProductForm = new FormGroup({
    name: new FormControl<string>(''),
    description: new FormControl<string>(''),
    price: new FormControl<number>(0),
    thumbnail: new FormControl<FileHandler | null>(this.thumbnail, [Validators.requiredTrue]),
    images: new FormControl<FileHandler[]>(this.images, [Validators.requiredTrue]),
  });

  onThumbnailSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      
      const image = input.files[0];
      const reader = new FileReader();
      reader.onload = (e) => {
        const fileHandler: FileHandler = {
          file: image,
          url: e.target?.result as string
        }

        this.thumbnail = fileHandler;
        this.addProductForm.patchValue({ thumbnail: fileHandler });
      };

      reader.readAsDataURL(image); 
    }
  }

  onImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      
      const image = input.files[0];
      
      const reader = new FileReader();
      reader.onload = (e) => {
        const fileHandler: FileHandler = {
          file: image,
          url: e.target?.result as string
        }

        this.images.push(fileHandler);
      };

      reader.readAsDataURL(image); 
    }
  }

  deleteImage(id: number) {
    this.images.splice(id, 1);
  }

  submit() {
    this.productService.addProduct(this.addProductForm.value).subscribe((response: any) => {

    });
  }
}
