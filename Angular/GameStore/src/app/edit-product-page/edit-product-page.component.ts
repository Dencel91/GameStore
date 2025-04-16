import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../interfaces/product';
import { ProductService } from '../services/product.service';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { FileHandler } from '../interfaces/FileHandler';
import { NgIf } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-edit-product-page',
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './edit-product-page.component.html',
  styleUrl: './edit-product-page.component.css'
})
export class EditProductPageComponent {

  constructor(private route: ActivatedRoute, private productService: ProductService, private http: HttpClient) { }

  thumbnail: FileHandler | null = null;
  images: FileHandler[] = [];
  newImages: FileHandler[] = [];
  removedImages: FileHandler[] = [];

  editProductForm = new FormGroup({
    name: new FormControl<string>(''),
    description: new FormControl<string>(''),
    price: new FormControl<number>(0),
    updatedThumbnail: new FormControl<FileHandler | null>(null),
    newImages: new FormControl<FileHandler[]>(this.newImages),
    removedImages: new FormControl<FileHandler[]>(this.removedImages),
  });

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const productId = Number(params.get('id'));

      this.productService.GetProductDetails(productId).subscribe(product => {

        // const file = this.urlToFile(product.thumbnailUrl, 'image.jpg', 'image/jpeg').then((file) => {
        //   this.thumbnail = {
        //     file: file,
        //     url: product.thumbnailUrl
        //   };
        // });
        // this.urlToFile2(product.thumbnailUrl, 'image.jpg', 'image/jpeg');




        this.images = product.images.map((imageUrl: string) => {
          return {
            file: {} as File,
            url: imageUrl
          };
        });

        this.editProductForm.patchValue({
          name: product.name,
          description: product.description,
          price: product.price,
        });

        // this.editProductForm.setValue({
        //   name: product.name,
        //   description: product.description,
        //   price: product.price,
        //   updatedThumbnail: null,
        //   newImages: null,
        //   removedImages: null
        // });

      });
    });
  }

  
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
        this.editProductForm.patchValue({ updatedThumbnail: fileHandler });
      };

      reader.readAsDataURL(image); 
    }
  }

  // async urlToFile(url: string, filename: string, mimeType?: string): Promise<File> {
  //   const response = await fetch(url);
  //   const blob = await response.blob();
  //   return new File([blob], filename, { type: mimeType || blob.type });
  // }

  // urlToFile2(url: string, filename: string, mimeType?: string): void {
    
  //   this.http.get(url, { responseType: 'blob' }).subscribe((blob) => {
  //     const file = new File([blob], filename, { type: mimeType || blob.type });
  //     this.thumbnail = {
  //       file: file,
  //       url: url
  //     };
  //   });
  // }

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
        this.newImages.push(fileHandler);

      };

      reader.readAsDataURL(image); 
    }
  }

  deleteImage(id: number) {
    this.images.splice(id, 1);
  }

  submit() {
    this.productService.UpdateProduct(this.editProductForm.value).subscribe((response: any) => {
      
    });
  }
}
