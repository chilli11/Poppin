import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class AccountIndexRoute extends Route {
	@service accountService;
	model() {
		return this.accountService.me();
	}
}