import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteButton } from './delete-button';

describe('DeleteButton', () => {
  let component: DeleteButton;
  let fixture: ComponentFixture<DeleteButton>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteButton]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeleteButton);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
