import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../services/product.service';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { FileHandler } from '../interfaces/FileHandler';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-edit-product-page',
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './edit-product-page.component.html',
  styleUrl: './edit-product-page.component.css'
})
export class EditProductPageComponent {

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private router: Router) { }

  thumbnail: FileHandler | null = null;
  existedImages: FileHandler[] = [];
  newImages: FileHandler[] = [];
  removedImages: string[] = [];

  editProductForm = new FormGroup({
    id: new FormControl<number>(0),
    name: new FormControl<string>(''),
    description: new FormControl<string>(''),
    price: new FormControl<number>(0),
    updatedThumbnail: new FormControl<FileHandler | null>(null),
    newImages: new FormControl<FileHandler[]>(this.newImages),
    removedImages: new FormControl<string[]>([]),
  });

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const productId = Number(params.get('id'));

      this.productService.GetProductDetails(productId).subscribe(product => {

        this.thumbnail = {
          file: {} as File,
          url: product.thumbnailUrl
        };

        this.existedImages = product.images.map((imageUrl: string) => {
          return {
            file: {} as File,
            url: imageUrl
          };
        });

        this.editProductForm.patchValue({
          id: product.id,
          name: product.name,
          description: product.description,
          price: product.price,
        });
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

        this.newImages.push(fileHandler);
      };

      reader.readAsDataURL(image); 
    }
  }

  deleteExistedImage(id: number) {
    this.removedImages.push(this.existedImages[id].url as string);
    this.existedImages.splice(id, 1);
  }

  deleteNewImage(id: number) {
    this.newImages.splice(id, 1);
  }

  submit() {
    this.editProductForm.patchValue({ removedImages: this.removedImages });
    this.productService.UpdateProduct(this.editProductForm.value).subscribe((response: any) => {
      this.router.navigate(['/product', response.id]);
    });
  }
}
