import { module, test } from 'qunit';
import { setupTest } from 'ember-qunit';

module('Unit | Route | account/login', function(hooks) {
  setupTest(hooks);

  test('it exists', function(assert) {
    let route = this.owner.lookup('route:account/login');
    assert.ok(route);
  });
});
