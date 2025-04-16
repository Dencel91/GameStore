import { Component } from '@angular/core';
import { ProductService } from '../services/product.service';
import { Product } from '../interfaces/product';
import { ProductTileComponent } from "../product-tile/product-tile.component";
import { InfiniteScrollDirective } from 'ngx-infinite-scroll';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-product-list',
  imports: [ProductTileComponent, InfiniteScrollDirective, LoadingComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent {
  constructor(private productService: ProductService) { }
  
  static readonly pageSize: number = 12;

  loading: boolean = false;

  products: Product[] = [];
  nextPageCursor: number = 0;
  allLoaded: boolean = false;

  ngOnInit() {
    this.getProducts();
  }

  getProducts() {
    this.loading = true;
    this.productService.GetProducts(this.nextPageCursor, ProductListComponent.pageSize).subscribe((data) => {
      let products: Product[] = data.products;
      this.products = this.products.concat(products);
      this.nextPageCursor = data.nextPageCursor;
      this.allLoaded = (data.NextPageCursor == 0 || products?.length < ProductListComponent.pageSize);
      this.loading = false;
   });
  }

  onScrollDown() {
    if (!this.loading && !this.allLoaded) {
      this.getProducts();
    }
  }
}
