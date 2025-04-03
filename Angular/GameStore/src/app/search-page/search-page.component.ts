import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../services/product.service';
import { Product } from '../interfaces/product';
import { SearchItemComponent } from "../search-item/search-item.component";

@Component({
  selector: 'app-search-page',
  imports: [SearchItemComponent],
  templateUrl: './search-page.component.html',
  styleUrl: './search-page.component.css'
})
export class SearchPageComponent {

  constructor(private route: ActivatedRoute, private productService: ProductService) { }

  products: Product[] = [];

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const term = params.get('term') ?? '';

      if (!term) {
        return;
      }
  
      this.productService.SearchProduct(term).subscribe((products: Product[]) => {
        this.products = products;
      });
    })
  }
}
