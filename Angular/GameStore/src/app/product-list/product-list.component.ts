import { Component } from '@angular/core';
import { ProductService } from '../services/product.service';
import { Product } from '../interfaces/product';
import { ProductTileComponent } from "../product-tile/product-tile.component";

@Component({
  selector: 'app-product-list',
  imports: [ProductTileComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent {

  products: Product[] = [];

  constructor(private productService: ProductService) { }

  ngOnInit() {
    this.productService.GetProducts().subscribe((data: Product[]) => {
       this.products = data;
    });
  }
}
