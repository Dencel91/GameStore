import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-product-page',
  imports: [ReactiveFormsModule],
  templateUrl: './add-product-page.component.html',
  styleUrl: './add-product-page.component.css'
})

export class AddProductPageComponent {

  images: string[] = ['', '', ''];
  
  addProductForm = new FormGroup({
    name: new FormControl<string>(''),
    description: new FormControl<string>(''),
    price: new FormControl<number>(0),
    thumbnailUrl: new FormControl<string>('', [Validators.requiredTrue]),
    images: new FormControl<string[]>(this.images, [Validators.requiredTrue]),
  });


  submit() {
  }
}
