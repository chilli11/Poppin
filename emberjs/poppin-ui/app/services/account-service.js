import Service, { inject as injectService } from '@ember/service';
import HttpResources from '../utils/http-resources';
import { tracked } from '@glimmer/tracking';
import { Promise } from 'rsvp';

export default class AccountService extends Service {
	@injectService apiService;
	@injectService session;

	@tracked authInfo = null;
	@tracked accountInfo = null;
	@tracked profile = null;
	@tracked vendors = null;
	@tracked favorites = null;
	@tracked hidden = null;

	isAuthenticated() {
		if (this.authInfo) {
			if (this.authInfo.authorized) return this.myProfile();
			return Promise.reject();
		}
		return this.apiService.request({
			resource: HttpResources.isAuthenticated
		}).then(() => this.authInfo = { authorized: true })
			.then(() => this.myProfile())
			.catch(() => this.authInfo = { authorized: false });
	}

	registerAccount(credentials) {
		return this.apiService.request({
			resource: HttpResources.registerAccount,
			body: credentials
		});
	}

	login(credentials) {
		return this.apiService.request({
			resource: HttpResources.login,
			body: credentials
		}).then((response) => {
			sessionStorage.setItem('poppin_jwt', response.token);
			this.apiService.jwt = response.token;
			this.authInfo = { authorized: true }
			return response.token;
		});
	}

	me() {
		return this.apiService.request({
			resource: HttpResources.myAccount,
		}).then(({ user }) => this.accountInfo = user);
	}

	myProfile() {
		return this.apiService.request({
			resource: HttpResources.myProfile
		}).then(({ user, vendors, favorites, hidden }) => {
			this.profile = user;
			this.vendors = vendors;
			this.favorites = favorites;
			this.hidden = hidden;
		});
	}

	logout() {
		this.apiService.jwt = null;
		this.authInfo = null;
		sessionStorage.removeItem('poppin_jwt');
		return Promise.resolve();
	}
}
