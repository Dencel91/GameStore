import { Component, inject, Input, signal, TemplateRef, ViewChild, WritableSignal } from '@angular/core';
import { ModalDismissReasons, NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Cart } from '../interfaces/cart';
import { Product } from '../interfaces/product';
import { CartService } from '../services/cart.service';
import { Router } from '@angular/router';
import { CartItemComponent } from "../cart-item/cart-item.component";

@Component({
  selector: 'app-add-to-cart-modal',
  imports: [CartItemComponent],
  templateUrl: './add-to-cart-modal.component.html',
  styleUrl: './add-to-cart-modal.component.css'
})
export class AddToCartModalComponent {
  @Input() product: Product | null = null;
  // @Input() title: string = "";
  @ViewChild('content', { static: true }) content!: TemplateRef<any>;
  modal: NgbModalRef | undefined;
  title: string = "";

  private modalService = inject(NgbModal);
  closeResult: WritableSignal<string> = signal('');
  cart: Cart | null = null;

  constructor(private cartService: CartService, private router: Router) {}
  
  open(title: string, cart: Cart) {
    this.title = title;
    this.cart = cart;
    
    this.modal = this.modalService.open(this.content, { centered: true });
    
    this.modal.result.then(
      	(result) => {
      		this.closeResult.set(`Closed with: ${result}`);
      	},
      	(reason) => {
      		this.closeResult.set(`Dismissed ${this.getDismissReason(reason)}`);
      	},
      );
	}

  private getDismissReason(reason: any): string {
    switch (reason) {
      case ModalDismissReasons.ESC:
        return 'by pressing ESC';
      case ModalDismissReasons.BACKDROP_CLICK:
        return 'by clicking on a backdrop';
      default:
        return `with: ${reason}`;
    }
  }

  handlecartUpdated(event: any) {
    this.modal?.close();
    }

  goToCart(){
    this.router.navigate(['/cart']);
    this.modal?.close();
  }
}
