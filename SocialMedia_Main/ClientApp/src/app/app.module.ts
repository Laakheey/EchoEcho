import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { SignupComponent } from './Authorization/signup/signup/signup.component';

import { ReactiveFormsModule } from '@angular/forms';
import { ToastModule } from "primeng/toast";

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from './Authorization/login/login.component';
import { LogoutComponent } from './Authorization/logout/logout.component';
import { AddPostComponent } from './Post/add-post/add-post.component';
import { ChatviewComponent } from './Message/chatview/chatview.component';
import { AuthGuard } from './guard/auth.guard';
import { UserProfileComponent } from './User/user-profile/user-profile.component';
import { PostViewComponent } from './Post/post-view/post-view.component';

import { firebaseConfig } from 'src/environments/firebase-config';
import {AngularFireModule} from '@angular/fire/compat';
import {AngularFireStorageModule} from '@angular/fire/compat/storage';

import { ModalModule } from 'ngx-bootstrap/modal';
import { EditUserDetailsComponent } from './User/edit-user-details/edit-user-details.component';
import { PageNotExistComponent } from './page-not-exist/page-not-exist.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    SignupComponent,
    LoginComponent,
    AddPostComponent,
    ChatviewComponent,
    UserProfileComponent,
    PostViewComponent,
    EditUserDetailsComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    ToastModule,
    ReactiveFormsModule,
    CommonModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full', canActivate:[AuthGuard] },
      { path: 'auth/signup', component: SignupComponent },
      { path: 'auth/login', component: LoginComponent },
      { path: 'auth/logout', component: LogoutComponent },
      { path: 'chatView', component: ChatviewComponent, canActivate:[AuthGuard] },
      { path: 'chatView/:chatHeadId', component: ChatviewComponent, canActivate:[AuthGuard] },
      { path: 'user/:userId', component: UserProfileComponent, canActivate:[AuthGuard] },
      { path: 'editUser', component: EditUserDetailsComponent, canActivate:[AuthGuard] },
      { path: '**', component: PageNotExistComponent },
    ]),
    AngularFireModule.initializeApp(firebaseConfig),
    AngularFireStorageModule,
    ModalModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
