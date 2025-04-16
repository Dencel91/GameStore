import { Component, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../services/product.service';
import { Product } from '../interfaces/product';
import { CartService } from '../services/cart.service';
import { NgbCarouselModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { AddToCartModalComponent } from "../add-to-cart-modal/add-to-cart-modal.component";
import { Cart } from '../interfaces/cart';
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';
import { LoadingComponent } from "../loading/loading.component";

@Component({
  selector: 'app-product-page',
  imports: [NgbCarouselModule, AddToCartModalComponent, CommonModule, LoadingComponent, LoadingComponent],
  templateUrl: './product-page.component.html',
  styleUrl: './product-page.component.css'
})
export class ProductPageComponent {
  @ViewChild(AddToCartModalComponent) cartModal!: AddToCartModalComponent;

  isLoading: boolean = true;
  product: Product | null = null;
  userProductInfo: any = null;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartService: CartService,
    public authService: AuthService,
    private userService: UserService) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      this.getProductDetails(id);
    });
  }

  getProductDetails(id: number) {
    this.productService.GetProductDetails(id).subscribe({
      next: (data: any) => {
        this.product = data;

        if (this.authService.isAuthenticated()) {
          this.userService.getUserProductInfo(id).subscribe((response: any) => {
            this.userProductInfo = response;
            this.isLoading = false;
          });
        }
        else {
          this.isLoading = false;
        }
      },
      error: (error: any) => {
        console.log('Cannot get product details', error);
        this.isLoading = false;
      }
    });
  }

  addToCart(id: number) {
    this.cartService.AddToCart(id).subscribe({
      next: (cart: Cart) => {
        this.showCartModal("Added to your cart!", cart);
      },
      error: (error: any) => {
        if (error.status == 400) {
          switch (error.error) {
            case 'Invalid cart id':
              this.cartService.cartId = 0;
              this.addToCart(id);
              break;
            case "Product already in cart":
              this.cartService.GetCart().subscribe((cart: Cart) => {
                this.showCartModal("Already in your cart!", cart);
              })
              break;
          }
        }
      }
    });
  }

  addToLibrary() {

  }

  showCartModal(title: string, cart: Cart) {
    this.cartModal.open(title, cart);
  }
}
