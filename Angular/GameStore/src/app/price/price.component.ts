import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-price',
  imports: [CommonModule],
  templateUrl: './price.component.html',
  styleUrl: './price.component.css'
})
export class PriceComponent {
  @Input() price: number = 0
}
