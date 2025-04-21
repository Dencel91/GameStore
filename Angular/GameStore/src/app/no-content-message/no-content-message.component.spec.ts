import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NoContentMessageComponent } from './no-content-message.component';

describe('NoContentMessageComponent', () => {
  let component: NoContentMessageComponent;
  let fixture: ComponentFixture<NoContentMessageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NoContentMessageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NoContentMessageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
