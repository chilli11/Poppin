import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class AccountRoute extends Route {
	@service accountService;
	beforeModel() {
		return this.accountService.isAuthenticated();
	}
}
