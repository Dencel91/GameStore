import { Component, inject, TemplateRef, ViewChild } from '@angular/core';
import { UserService } from '../services/user.service';
import { Product } from '../interfaces/product';
import { RouterLink } from '@angular/router';
import { LoadingComponent } from "../loading/loading.component";
import { NoContentMessageComponent } from "../no-content-message/no-content-message.component";
import { NgClass } from '@angular/common';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-library-page',
  imports: [RouterLink, LoadingComponent, NoContentMessageComponent, NgClass],
  templateUrl: './library-page.component.html',
  styleUrl: './library-page.component.css'
})
export class LibraryPageComponent {

  constructor(private userService: UserService) { }

  private offcanvasService = inject(NgbOffcanvas);
  @ViewChild('productPanelContent', { static: true }) productPanelContent!: TemplateRef<any>;
  productPanel: any;

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

    if (this.productPanel) {
      this.productPanel.close();
      this.productPanel = null;
    }
  }

  showProductPanel() {
    this.productPanel = this.offcanvasService.open(this.productPanelContent);
  }
}
