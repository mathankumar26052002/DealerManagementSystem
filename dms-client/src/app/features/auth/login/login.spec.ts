import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { vi } from 'vitest';

import { Login } from './login';
import { AuthService } from '../../../core/services/auth';

describe('Login', () => {

  let component: Login;
  let fixture: ComponentFixture<Login>;

  const authServiceMock = {
    login: vi.fn()
  };

  const routerMock = {
    navigate: vi.fn()
  };

  beforeEach(async () => {

    await TestBed.configureTestingModule({
      imports: [Login],

      providers: [
        {
          provide: AuthService,
          useValue: authServiceMock
        },
        {
          provide: Router,
          useValue: routerMock
        }
      ]

    }).compileComponents();

    fixture = TestBed.createComponent(Login);

    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {

    expect(component).toBeTruthy();

  });

});