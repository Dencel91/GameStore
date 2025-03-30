import { Component } from '@angular/core';
import { UserService } from '../services/user.service';
import { Product } from '../interfaces/product';

@Component({
  selector: 'app-library-page',
  imports: [],
  templateUrl: './library-page.component.html',
  styleUrl: './library-page.component.css'
})
export class LibraryPageComponent {
  constructor(private userService: UserService) { }

  products: Product[] = [];

  ngOnInit() {
    this.userService.getUserProducts().subscribe((products: Product[]) => {
      this.products = products;
    });
  }
}
