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

  constructor(private productService: ProductService) { }
  
  static readonly pageSize: number = 12;

  products: Product[] = [];
  nextPageCursor: number = 0;
  allProductsLoaded: boolean = false;

  ngOnInit() {
    this.getProducts();
  }

  getProducts() {
    this.productService.GetProducts(this.nextPageCursor, ProductListComponent.pageSize).subscribe((data) => {
      let products: Product[] = data.products;
      this.products = this.products.concat(products);
      this.nextPageCursor = data.nextPageCursor;
      this.allProductsLoaded = (data.NextPageCursor == 0 || products?.length < ProductListComponent.pageSize);
   });
  }
}
