import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class VendorsIndexRoute extends Route {
	@service vendorsService;
	model() {
		return this.vendorsService.getAllVendors();
	}
}
