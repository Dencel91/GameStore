import { Component } from '@angular/core';
import { UserService } from '../services/user.service';
import { Product } from '../interfaces/product';
import { RouterLink } from '@angular/router';
import { LoadingComponent } from "../loading/loading.component";
import { NoContentMessageComponent } from "../no-content-message/no-content-message.component";

@Component({
  selector: 'app-library-page',
  imports: [RouterLink, LoadingComponent, NoContentMessageComponent],
  templateUrl: './library-page.component.html',
  styleUrl: './library-page.component.css'
})
export class LibraryPageComponent {

  constructor(private userService: UserService) { }

  loading = true;
  products: Product[] = [];
  selectedProduct: Product | null = null;

  ngOnInit() {
    this.userService.getUserProducts().subscribe((products: Product[]) => {
      this.products = products;
      if (this.products.length > 0) {
        this.selectedProduct = this.products[0];
      }
      
      this.loading = false;
    });
  }

  selectProduct(product: Product) {
    this.selectedProduct = product;
  }
}
