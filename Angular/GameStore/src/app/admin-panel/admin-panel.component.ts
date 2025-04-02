import { Component, inject, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-admin-panel',
  imports: [],
  templateUrl: './admin-panel.component.html',
  styleUrl: './admin-panel.component.css'
})
export class AdminPanelComponent {

  constructor(private router: Router) {}

  @ViewChild('offCanvasContent', { static: true }) offCanvasContent!: TemplateRef<any>;
  private offcanvasService = inject(NgbOffcanvas);

  currentRouteParams: any;
  currentRoute: any;
  offcanvas: any;

  open() {
    const parts = this.router.url.split('/');
    
    this.currentRoute = parts[1];
    this.currentRouteParams = parts[2];

		this.offcanvas = this.offcanvasService.open(this.offCanvasContent);
	}

  close() {
    this.offcanvas.close();
  }

  AddProduct() {
    this.router.navigate(['/add-product']);
    this.close();
  }

  EditProduct() {
    this.router.navigate(['/edit-product', this.currentRouteParams]);
    this.close();
  }
}
