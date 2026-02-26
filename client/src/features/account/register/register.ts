import { Component, inject, OnInit, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AccountService } from '../../../core/services/account-service';
import { RegisterCreds } from '../../../types/user';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, JsonPipe],  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private accountService = inject(AccountService);
  protected creds = {} as RegisterCreds;

  protected registerForm: FormGroup = new FormGroup({});
  cancelRegister = output<boolean>();

    private readonly EMAIL_REGEX = '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$';

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      email: new FormControl('',
        [Validators.required, Validators.pattern(this.EMAIL_REGEX)]),
      displayName: new FormControl('',
        [Validators.required]),
      password: new FormControl('',
        [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      confirmPassword: new FormControl('',
        [Validators.required])
    })
  }

  register(): void {
    console.group("REGISTER");
    console.log(this.registerForm.value);
    console.groupEnd();
  }

  cancel(): void {
    this.cancelRegister.emit(false);
  }
}
