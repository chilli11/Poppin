import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class AccountIndexRoute extends Route {
	@service accountService;
	get authorized() {
		return this.accountService.authInfo && this.accountService.authInfo.authorized;
	}
	model() {
		return this.authorized ? this.accountService.me() : null;
	}
}
