import { Component, inject, output, signal  } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../../../core/services/account-service';
import { RegisterCreds } from '../../../types/user';
import { JsonPipe } from '@angular/common';
import { TextInput } from "../../../shared/text-input/text-input";
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, TextInput],  
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private accountService = inject(AccountService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  protected creds = {} as RegisterCreds;

  protected credentialsForm: FormGroup;
  protected profileForm: FormGroup;
  protected currentStep = signal(1);  cancelRegister = output<boolean>();
  protected validationErrors = signal<string[]>([]);

   constructor() {
   this.credentialsForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      displayName: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });
    this.profileForm = this.fb.group({
      gender: ['', [Validators.required]],
      birthDay: ['', [Validators.required]],
      city: ['', [Validators.required]],
      country: ['', [Validators.required]]
    });


    this.credentialsForm.controls['password'].valueChanges.subscribe(() => {
      this.credentialsForm.controls['confirmPassword'].updateValueAndValidity();
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const parent = control.parent;
      if (!parent) return null;
      const matchValue = parent.get(matchTo)?.value;
      return control.value === matchValue ? null : { passwordMismatch: true };
    };
  }

  nextStep() {
    if (this.credentialsForm.valid) {
      this.currentStep.update(prevStep => prevStep + 1);
    }
  }

  prevStep() {
    this.currentStep.update(prevStep => prevStep - 1);
  }

  getMaxDate() {
    const today = new Date();
    today.setFullYear(today.getFullYear() - 18);
    return today.toISOString().split('T')[0];
  }

  register(): void {
    if (this.credentialsForm.valid && this.profileForm.valid) {
      const formData = { ...this.credentialsForm.value, ...this.profileForm.value };

      this.accountService.register(formData).subscribe({
        next: () => {
          this.router.navigateByUrl('/members');
        },
        error: error => {
          this.validationErrors.set(error);
          console.log(error);
        }
      });
    }
  }

  cancel(): void {
    this.cancelRegister.emit(false);
  }
}
