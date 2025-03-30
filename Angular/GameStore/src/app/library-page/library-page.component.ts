import { Component } from '@angular/core';
import { UserService } from '../services/user.service';
import { Product } from '../interfaces/product';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-library-page',
  imports: [RouterLink],
  templateUrl: './library-page.component.html',
  styleUrl: './library-page.component.css'
})
export class LibraryPageComponent {

  constructor(private userService: UserService) { }

  products: Product[] = [];

  selectedProduct: Product | null = null;

  ngOnInit() {
    this.userService.getUserProducts().subscribe((products: Product[]) => {
      this.products = products;
    });
  }

  selectProduct(product: Product) {
    this.selectedProduct = product;
  }
}
