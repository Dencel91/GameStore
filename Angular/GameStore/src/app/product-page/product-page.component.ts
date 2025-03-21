import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../services/product.service';
import { Product } from '../interfaces/product';
import { CartService } from '../services/cart.service';
import { NgbCarouselModule } from '@ng-bootstrap/ng-bootstrap';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-product-page',
  imports: [NgbCarouselModule, NgIf],
  templateUrl: './product-page.component.html',
  styleUrl: './product-page.component.css'
})
export class ProductPageComponent {
  isLoading: boolean = true;
  product: Product | null = null;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private carttService: CartService) {}

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));

      // this.productService.GetProductDetails(id).subscribe((data: Product) => {
      //   setTimeout(() => {
      //     this.product = data;
      //     this.isLoading = false;
      //   }, 3000);
        
      //   console.log('Product:', data);
      // },(error: any) => {
      //   this.isLoading = false;
      //   console.log('Cannot get product details', error);
      // });

      this.productService.GetProductDetails(id).subscribe({
        next: (data: any) =>{
          this.product = data;
          this.isLoading = false;
        },
        error: (error: any) => {
          console.log('Cannot get product details', error);
          this.isLoading = false;
        }})
    }); 
  }

  addToCart(id: Number) {
    this.carttService.AddToCart(id);
    console.log('Product added to cart:', id);
  }
}
