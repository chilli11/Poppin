import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';
import { action } from '@ember/object';

export default class AccountIndexRoute extends Route {
	@service accountService;
	model() {
		return {
			profile: this.accountService.profile,
			vendors: this.accountService.vendors,
			favorites: this.accountService.favorites,
			hidden: this.accountService.hidden
		};
	}

	afterModel() {
		if (!this.accountService.authInfo || !this.accountService.authInfo.authorized) {
			return this.transitionTo('account.login');
		}
	}

	@action
	refreshRoute() {
		return this.refresh();
	}
}