import { NgClass } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-loading',
  imports: [NgClass],
  templateUrl: './loading.component.html',
  styleUrl: './loading.component.css'
})
export class LoadingComponent {
  @Input() loading: boolean = false;
  @Input() loadingClass: string = '';

  ngOnInit() {
    if (!this.loadingClass) {
      this.loadingClass = 'position-absolute top-50 start-50 translate-middle';
    }
  }
}
