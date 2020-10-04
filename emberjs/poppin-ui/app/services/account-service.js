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
		}).then(() => this.myProfile());
	}

	me() {
		if (!this.authInfo || !this.authInfo.authorized)
			return Promise.reject({ errors: ['Unauthorized'] });

		return this.apiService.request({
			resource: HttpResources.myAccount,
		}).then(({ user }) => this.accountInfo = user);
	}

	myProfile() {
		if (!this.authInfo || !this.authInfo.authorized)
			return Promise.reject({ errors: ['Unauthorized'] });

			return this.apiService.request({
			resource: HttpResources.myProfile
		}).then(({ user, vendors, favorites, hidden }) => {
			this.profile = user;
			this.vendors = vendors;
			this.favorites = favorites;
			this.hidden = hidden;
		});
	}

	addFavorite(locId) {
		return this.apiService.request({
			resource: HttpResources.addFavorite,
			body: { locId }
		}).then(() => {
			this.profile.favorites.push(locId);
			return this.profile;
		});
	}

	removeFavorite(locId) {
		return this.apiService.request({
			resource: HttpResources.removeFavorite,
			body: { locId }
		});
	}

	logout() {
		this.clearUserData();
		return Promise.resolve();
	}

	clearUserData() {
		this.apiService.jwt = null;
		this.authInfo = null;
		this.accountInfo = null;
		this.profile = null;
		this.vendors = null;
		this.favorites = null;
		this.hidden = null;
		sessionStorage.removeItem('poppin_jwt');
	}
}
